using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

/// <summary>
/// Raised when the station AI uses the "Control Shell" action to open the list of AI shells.
/// </summary>
public sealed partial class StationAiControlShellActionEvent : InstantActionEvent
{
}

/// <summary>
/// Raised on an AI shell when its occupant uses the "Return to Core" action. Ends the AI's visit
/// and returns it to its core.
/// </summary>
public sealed partial class StationAiShellReturnEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public enum StationAiControlShellUiKey : byte
{
    Key,
}

/// <summary>
/// A single AI shell entry shown in the "Control Shell" window.
/// </summary>
[Serializable, NetSerializable]
public struct StationAiShellData
{
    public NetEntity Entity;
    public string Name;

    /// <summary>
    /// False if the shell is already controlled (by the AI or anyone else) or otherwise unusable.
    /// </summary>
    public bool Available;

    public StationAiShellData(NetEntity entity, string name, bool available)
    {
        Entity = entity;
        Name = name;
        Available = available;
    }
}

[Serializable, NetSerializable]
public sealed class StationAiControlShellBuiState : BoundUserInterfaceState
{
    public List<StationAiShellData> Shells;

    public StationAiControlShellBuiState(List<StationAiShellData> shells)
    {
        Shells = shells;
    }
}

/// <summary>
/// Sent from client to server when the AI selects a shell to control.
/// </summary>
[Serializable, NetSerializable]
public sealed class StationAiControlShellMessage : BoundUserInterfaceMessage
{
    public NetEntity Target;

    public StationAiControlShellMessage(NetEntity target)
    {
        Target = target;
    }
}
