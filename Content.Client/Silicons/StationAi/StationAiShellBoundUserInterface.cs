using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;

namespace Content.Client.Silicons.StationAi;

public sealed class StationAiShellBoundUserInterface : BoundUserInterface
{
    private StationAiShellWindow? _window;

    public StationAiShellBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<StationAiShellWindow>();
        _window.ShellClicked += OnShellClicked;
    }

    private void OnShellClicked(NetEntity target)
    {
        SendMessage(new StationAiControlShellMessage(target));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not StationAiControlShellBuiState cast)
            return;

        _window?.UpdateShells(cast.Shells);
        _window?.Populate();
    }
}
