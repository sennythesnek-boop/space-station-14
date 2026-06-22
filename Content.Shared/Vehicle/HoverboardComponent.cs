using Robust.Shared.GameStates;

namespace Content.Shared.Vehicle;

/// <summary>
/// A drivable hoverboard. A rider buckles onto it and their WASD input is relayed to the board, where
/// <see cref="Systems.SharedMoverController"/> reinterprets it as car-like controls: W/S thrust forward/reverse
/// along the board's facing, A/D rotate the board, with acceleration up to a max speed.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HoverboardComponent : Component
{
    /// <summary>How fast the forward speed ramps up, in tiles/sec².</summary>
    [DataField]
    public float Acceleration = 4f;

    /// <summary>Top forward speed in tiles/sec (~2x the 2.5 base walk speed).</summary>
    [DataField]
    public float MaxSpeed = 5f;

    /// <summary>Top reverse speed in tiles/sec.</summary>
    [DataField]
    public float ReverseMaxSpeed = 2.5f;

    /// <summary>Turn rate in radians/sec while A/D is held.</summary>
    [DataField]
    public float RotationSpeed = 2.6f;

    /// <summary>How fast it coasts to a stop when no thrust is applied, in tiles/sec².</summary>
    [DataField]
    public float Friction = 5f;
}
