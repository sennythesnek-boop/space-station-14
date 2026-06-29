using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Administration.UI.CombatStats;

[UsedImplicitly]
public sealed class CombatStatsEui : BaseEui
{
    private readonly CombatStatsWindow _window;

    public CombatStatsEui()
    {
        _window = new CombatStatsWindow();
        _window.OnRefresh += () => SendMessage(new CombatStatsRefreshMessage());
        _window.OnSelectRound += round => SendMessage(new CombatStatsSelectRoundMessage(round));
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is CombatStatsEuiState s)
            _window.SetState(s);
    }

    public override void Opened()
    {
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }
}
