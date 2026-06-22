using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Silicons.StationAi;

/// <summary>
/// A brain-slot item (like the positronic brain or MMI) that, when installed into a cyborg chassis,
/// makes that cyborg something the station AI can remotely inhabit (it gains
/// <see cref="AiRemoteControllableComponent"/> while the core is installed).
/// Unlike the positronic brain it is not a ghost role and carries no mind of its own.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AiVesselCoreComponent : Component
{
}
