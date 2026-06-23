using Content.Server.Administration.Managers;
using Content.Server.Administration.RoundStats;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;

namespace Content.Server.Administration;

/// <summary>Server side of the combat stats admin window (<c>combatstats</c>). Read-only.</summary>
public sealed partial class CombatStatsEui : BaseEui
{
    [Dependency] private IAdminManager _admins = default!;
    [Dependency] private IEntityManager _entity = default!;

    private CombatStatsEuiState _state = new(new());

    public CombatStatsEui()
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
            BuildState();
    }

    private bool CanView() => _admins.HasAdminFlag(Player, AdminFlags.Logs);

    public override EuiStateBase GetNewState() => _state;

    public void BuildState()
    {
        if (!CanView())
        {
            Close();
            return;
        }

        _state = new CombatStatsEuiState(_entity.System<RoundStatsSystem>().BuildEntries());
        StateDirty();
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is CombatStatsRefreshMessage)
            BuildState();
    }
}
