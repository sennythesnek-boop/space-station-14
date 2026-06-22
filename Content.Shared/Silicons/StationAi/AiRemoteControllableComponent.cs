using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Silicons.StationAi;

/// <summary>
/// Marks an entity (typically a purpose-built "AI shell" cyborg chassis) as something the
/// station AI can remotely take control of via its "Control Shell" action.
/// Only the AI may inhabit these; they are never offered as a ghost role.
/// The AI inhabits the shell by mind-visiting it, so its core stays intact and it can return.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AiRemoteControllableComponent : Component
{
    /// <summary>
    /// The "Return to Core" action granted to whoever is currently inhabiting this shell.
    /// Tracked so it can be removed again when the AI leaves. Server-only.
    /// </summary>
    public EntityUid? ReturnAction;
}
