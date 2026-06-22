using Content.Shared.Actions;
using Content.Shared.Ghost;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

/// <summary>
/// Raised when the station AI uses the "Warp" action to open the warp list window.
/// </summary>
public sealed partial class StationAiWarpActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public enum StationAiWarpUiKey : byte
{
    Key,
}

/// <summary>
/// Sent from server to client to populate the AI warp window with the available warp targets.
/// Reuses the ghost <see cref="GhostWarp"/> struct (warp points + alive/critical players).
/// </summary>
[Serializable, NetSerializable]
public sealed class StationAiWarpBuiState : BoundUserInterfaceState
{
    public List<GhostWarp> Warps;

    public StationAiWarpBuiState(List<GhostWarp> warps)
    {
        Warps = warps;
    }
}

/// <summary>
/// Sent from client to server when the AI picks a warp target. Moves the AI eye next to the target.
/// </summary>
[Serializable, NetSerializable]
public sealed class StationAiWarpToTargetMessage : BoundUserInterfaceMessage
{
    public NetEntity Target;

    public StationAiWarpToTargetMessage(NetEntity target)
    {
        Target = target;
    }
}
