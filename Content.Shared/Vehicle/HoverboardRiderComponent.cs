namespace Content.Shared.Vehicle;

/// <summary>
/// Added to a player while they are buckled onto a hoverboard. Buckling normally forces CanMove off, which would
/// stop their input from being relayed to the board; this marker lets <see cref="SharedHoverboardSystem"/> re-enable
/// movement (the rider's input is forwarded to the board, they don't walk off it) so they can actually drive.
/// </summary>
[RegisterComponent]
public sealed partial class HoverboardRiderComponent : Component
{
}
