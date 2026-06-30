using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Database;
using Content.Shared.Eui;

namespace Content.Server.Administration;

/// <summary>
/// Server side of the migration oversight tool (<c>migrations</c>). Shows the migration log, lets admins
/// approve/reject pending (name-only) auto-migrations, and manually transfer data from one player to
/// another. All data movement is delegated to <see cref="UserMigrationManager"/>.
/// </summary>
public sealed partial class MigrationsEui : BaseEui
{
#pragma warning disable IDE0044 // injected by [Dependency]
    [Dependency] private IAdminManager _admins = default!;
    [Dependency] private IServerDbManager _db = default!;
    [Dependency] private IPlayerLocator _locator = default!;
    [Dependency] private UserMigrationManager _migration = default!;
    [Dependency] private IAdminLogManager _adminLog = default!;
#pragma warning restore IDE0044

    private const int HistoryLimit = 200;

    private List<MigrationRow> _rows = [];
    private string? _lastResult;

    public MigrationsEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override void Opened()
    {
        base.Opened();
        _admins.OnPermsChanged += OnPermsChanged;
    }

    public override void Closed()
    {
        base.Closed();
        _admins.OnPermsChanged -= OnPermsChanged;
    }

    private void OnPermsChanged(AdminPermsChangedEventArgs args)
    {
        if (args.Player == Player)
            Load();
    }

    public override EuiStateBase GetNewState()
    {
        return new MigrationsEuiState(_lastResult, CanMigrateAdmin(), _rows);
    }

    private bool CanUse()
    {
        return _admins.HasAdminFlag(Player, AdminFlags.Moderator);
    }

    /// <summary>Migrating admin rank/flags is gated behind the Permissions flag to prevent escalation.</summary>
    private bool CanMigrateAdmin()
    {
        return _admins.HasAdminFlag(Player, AdminFlags.Permissions);
    }

    public async void Load()
    {
        if (!CanUse())
        {
            Close();
            return;
        }

        var logs = await _db.GetMigrationLogsAsync(HistoryLimit);
        _rows = logs.Select(Map).ToList();
        StateDirty();
    }

    private static MigrationRow Map(MigrationLog log)
    {
        return new MigrationRow(
            log.Id,
            log.SourceUserId.ToString(),
            log.TargetUserId.ToString(),
            log.SourceUserName,
            log.TargetUserName,
            log.Time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            log.Automatic,
            log.Status == MigrationStatus.Pending,
            log.Status.ToString(),
            log.Scope.ToString(),
            log.MatchReason,
            log.Detail ?? string.Empty);
    }

    /// <summary>
    /// Resolve a name-or-GUID input to a player. If the input is a username (not a GUID) and more than one
    /// account has ever used that name, this refuses with an "ambiguous" error so the admin uses the GUID
    /// instead — otherwise the locator would silently pick the most recently seen account, which is exactly
    /// the wrong-account hazard this whole feature exists to avoid.
    /// </summary>
    private async Task<(LocatedPlayerData? Data, string? Error)> ResolveAsync(string input)
    {
        var located = await _locator.LookupIdByNameOrIdAsync(input);
        if (located == null)
            return (null, Loc.GetString("migrations-error-resolve", ("input", input)));

        if (!Guid.TryParse(input.Trim(), out _))
        {
            var matches = await _db.GetPlayerRecordsByUserNameAsync(input.Trim());
            if (matches.Count > 1)
                return (null, Loc.GetString("migrations-error-ambiguous",
                    ("name", input.Trim()), ("count", matches.Count)));
        }

        return (located, null);
    }

    public override async void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (!CanUse())
            return;

        switch (msg)
        {
            case MigrationApproveMessage approve:
            {
                var result = await _migration.ApprovePendingAsync(approve.Id, Player.UserId);
                _lastResult = result.Message;
                if (result.Success)
                    _adminLog.Add(LogType.Action, LogImpact.High,
                        $"{Player:actor} approved data migration #{approve.Id}: {result.Message}");
                Load();
                break;
            }
            case MigrationRejectMessage reject:
            {
                var result = await _migration.RejectPendingAsync(reject.Id, Player.UserId);
                _lastResult = result.Message;
                Load();
                break;
            }
            case MigrationManualMessage manual:
            {
                var (source, sourceError) = await ResolveAsync(manual.Source);
                var (target, targetError) = await ResolveAsync(manual.Target);
                if (source == null || target == null)
                {
                    _lastResult = sourceError ?? targetError;
                    StateDirty();
                    return;
                }

                var scope = (MigrationScope) manual.Scope;
                if ((scope & MigrationScope.Admin) != 0 && !CanMigrateAdmin())
                {
                    _lastResult = Loc.GetString("migrations-error-admin-perm");
                    StateDirty();
                    return;
                }

                var result = await _migration.PerformManualAsync(source, target, scope, manual.Merge, Player.UserId);
                _lastResult = result.Message;
                if (result.Success)
                    _adminLog.Add(LogType.Action, LogImpact.High,
                        $"{Player:actor} migrated data from {source.Username} ({source.UserId}) to {target.Username} ({target.UserId}) [{scope}, {(manual.Merge ? "merge" : "replace")}]: {result.Message}");
                Load();
                break;
            }
        }
    }
}
