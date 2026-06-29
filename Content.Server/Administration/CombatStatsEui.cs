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

    private CombatStatsEuiState _state = new(0, 0, new(), new());

    private int? _selectedRound;

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

        var system = _entity.System<RoundStatsSystem>();
        var current = system.CurrentRoundId;
        var available = system.GetAvailableRounds();
        var selected = _selectedRound ?? current;

        // If the selected round is no longer available (e.g. its file was removed), fall back to current.
        if (!available.Contains(selected))
            selected = current;

        _state = new CombatStatsEuiState(current, selected, available, system.GetRoundStats(selected));
        StateDirty();
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        switch (msg)
        {
            case CombatStatsSelectRoundMessage select:
                _selectedRound = select.RoundId;
                BuildState();
                break;
            case CombatStatsRefreshMessage:
                BuildState();
                break;
        }
    }
}
