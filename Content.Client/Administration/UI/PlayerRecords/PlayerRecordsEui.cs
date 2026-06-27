using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Administration.UI.PlayerRecords;

[UsedImplicitly]
public sealed class PlayerRecordsEui : BaseEui
{
    private readonly PlayerRecordsWindow _window;

    public PlayerRecordsEui()
    {
        _window = new PlayerRecordsWindow();
        _window.OnRequest += (filter, page) => SendMessage(new PlayerRecordsRequestMessage(filter, page));
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is PlayerRecordsEuiState s)
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
