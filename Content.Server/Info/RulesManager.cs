using System.Globalization;
using System.Net;
using Content.Server.Administration.Logs;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Info;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Server.Info;

public sealed partial class RulesManager
{
    [Dependency] private IServerDbManager _dbManager = default!;
    [Dependency] private INetManager _netManager = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IAdminLogManager _adminLog = default!;
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;

    private static DateTime LastValidReadTime => DateTime.UtcNow - TimeSpan.FromDays(60);

    public void Initialize()
    {
        _sawmill = _logManager.GetSawmill("rules");

        _netManager.Connected += OnConnected;
        _netManager.RegisterNetMessage<SendRulesInformationMessage>();
        _netManager.RegisterNetMessage<RulesAcceptedMessage>(OnRulesAccepted);
    }

    private async void OnConnected(object? sender, NetChannelArgs e)
    {
        var isLocalhost = IPAddress.IsLoopback(e.Channel.RemoteEndPoint.Address) &&
                            _cfg.GetCVar(CCVars.RulesExemptLocal);

        var lastRead = await _dbManager.GetLastReadRules(e.Channel.UserId);

        // The player only skips the rules popup if they accepted them recently AND after the
        // most recent rules update. Bumping rules.last_updated re-prompts everyone on connect.
        var hasCooldown = lastRead > LastValidReadTime && HasReadCurrentRules(lastRead);

        var showRulesMessage = new SendRulesInformationMessage
        {
            PopupTime = _cfg.GetCVar(CCVars.RulesWaitTime),
            CoreRules = _cfg.GetCVar(CCVars.RulesFile),
            ShouldShowRules = !isLocalhost && !hasCooldown,
        };
        _netManager.ServerSendMessage(showRulesMessage, e.Channel);
    }

    /// <summary>
    /// Returns true if <paramref name="lastRead"/> is at or after the configured rules.last_updated date.
    /// If the CVar is unset (or unparseable) version-gating is disabled and this always returns true.
    /// </summary>
    private bool HasReadCurrentRules(DateTimeOffset? lastRead)
    {
        var raw = _cfg.GetCVar(CCVars.RulesLastUpdated);
        if (string.IsNullOrWhiteSpace(raw))
            return true;

        if (!DateTime.TryParse(
                raw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var rulesUpdated))
        {
            _sawmill.Warning($"Could not parse rules.last_updated value '{raw}'. Skipping rules version check.");
            return true;
        }

        return lastRead != null && lastRead.Value.UtcDateTime >= rulesUpdated;
    }

    private async void OnRulesAccepted(RulesAcceptedMessage message)
    {
        var date = DateTime.UtcNow;
        await _dbManager.SetLastReadRules(message.MsgChannel.UserId, date);
        if (message.FuckRules && _player.TryGetSessionById(message.MsgChannel.UserId, out var session))
            _adminLog.Add(LogType.Connection, LogImpact.Extreme, $"Player {session} used the fuckrules command.");
    }
}
