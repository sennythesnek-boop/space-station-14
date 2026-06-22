using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;

namespace Content.Client.Silicons.StationAi;

public sealed class StationAiWarpBoundUserInterface : BoundUserInterface
{
    private StationAiWarpWindow? _window;

    public StationAiWarpBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<StationAiWarpWindow>();
        _window.WarpClicked += OnWarpClicked;
    }

    private void OnWarpClicked(NetEntity target)
    {
        SendMessage(new StationAiWarpToTargetMessage(target));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not StationAiWarpBuiState cast)
            return;

        _window?.UpdateWarps(cast.Warps);
        _window?.Populate();
    }
}
