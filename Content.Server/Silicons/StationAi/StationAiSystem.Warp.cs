using Content.Shared.Silicons.StationAi;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Server.Silicons.StationAi;

public sealed partial class StationAiSystem
{
    [Dependency] private UserInterfaceSystem _ui = default!;

    private void InitializeWarp()
    {
        SubscribeLocalEvent<StationAiHeldComponent, StationAiWarpActionEvent>(OnWarpAction);
        SubscribeLocalEvent<StationAiHeldComponent, BoundUIOpenedEvent>(OnAiBuiOpened);
        SubscribeLocalEvent<StationAiHeldComponent, StationAiWarpToTargetMessage>(OnWarpToTarget);
    }

    private void OnWarpAction(Entity<StationAiHeldComponent> ent, ref StationAiWarpActionEvent args)
    {
        if (args.Handled || !TryComp<ActorComponent>(ent, out var actor))
            return;

        args.Handled = true;
        _ui.TryToggleUi(ent.Owner, StationAiWarpUiKey.Key, actor.PlayerSession);
    }

    private void OnAiBuiOpened(Entity<StationAiHeldComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (args.UiKey.Equals(StationAiWarpUiKey.Key))
        {
            var warps = _ghost.GetWarps(ent.Owner);
            _ui.SetUiState(ent.Owner, StationAiWarpUiKey.Key, new StationAiWarpBuiState(warps));
        }
        else if (args.UiKey.Equals(StationAiControlShellUiKey.Key))
        {
            _ui.SetUiState(ent.Owner, StationAiControlShellUiKey.Key, new StationAiControlShellBuiState(GetShellList()));
        }
    }

    private void OnWarpToTarget(Entity<StationAiHeldComponent> ent, ref StationAiWarpToTargetMessage args)
    {
        if (!TryGetCore(ent.Owner, out var core) || core.Comp?.RemoteEntity == null)
            return;

        if (!TryGetEntity(args.Target, out var target) || !Exists(target))
            return;

        // The AI itself never moves; we relocate its remote eye next to the chosen target,
        // mirroring how "Jump to core" repositions the eye.
        _xforms.DropNextTo(core.Comp.RemoteEntity.Value, target.Value);
    }
}
