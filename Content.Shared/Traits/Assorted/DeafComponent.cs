using Robust.Shared.GameStates;

namespace Content.Shared.Traits.Assorted;

/// <summary>
/// Makes an entity completely deaf: it cannot hear any local speech, whispers or radio, including its own voice.
/// On the client this also mutes all audio for the local player while the component is present.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DeafComponent : Component;
