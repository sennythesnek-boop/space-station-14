using Robust.Shared.GameStates;

namespace Content.Shared.Traits.Assorted;

/// <summary>
/// Makes an entity hard of hearing. It only understands spoken messages (say and whisper) clearly when the speaker is
/// very close. Past <see cref="ClearRange"/> the message is garbled ("h~~l~ w~~~~"), and past <see cref="MuffledRange"/>
/// it is not heard at all. In effect, all normal speech is perceived the way everyone else perceives a whisper.
/// Radio is unaffected, as it is fed directly into the ear.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HardOfHearingComponent : Component
{
    /// <summary>
    /// Within this distance (world units) speech is heard perfectly. Defaults to the whisper-clear range.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float ClearRange = 2f;

    /// <summary>
    /// Within this distance (but past <see cref="ClearRange"/>) speech is garbled. Past it, nothing is heard.
    /// Defaults to the whisper-muffled range.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float MuffledRange = 5f;

    /// <summary>
    /// Fraction of characters that survive obfuscation when a message is garbled (0 = all hidden, 1 = fully legible).
    /// Higher than a distant whisper (0.2) so normal speech is "hard to make out" rather than "barely audible".
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Clarity = 0.5f;
}
