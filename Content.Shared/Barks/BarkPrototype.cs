using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Barks;

/// <summary>
/// A "bark" voice: a collection of short sounds that are played procedurally,
/// one per character, while an entity speaks. Ported from Goob-Station.
/// </summary>
[Prototype("bark")]
public sealed partial class BarkPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;

    /// <summary>
    /// Localized display name shown in the character editor.
    /// </summary>
    [DataField(required: true)]
    public string Name = string.Empty;

    /// <summary>
    /// The collection of sound files that are used for the gibberish bark voice.
    /// </summary>
    [DataField(required: true)]
    public SoundCollectionSpecifier? SoundCollection;

    /// <summary>
    /// The neural-TTS speaker name this voice maps to when "read aloud" mode is on.
    /// Must match a voice the configured TTS backend understands. Null falls back to
    /// the server's <c>tts.default_speaker</c> CVar.
    /// </summary>
    [DataField]
    public string? Speaker;

    /// <summary>
    /// A list of species that can use this bark. Null means everyone can.
    /// </summary>
    [DataField]
    public HashSet<string>? SpeciesWhitelist;

    /// <summary>
    /// The lower bound of the pitch variation.
    /// </summary>
    [DataField]
    public float MinPitch = 0.9f;

    /// <summary>
    /// The upper bound of the pitch variation.
    /// </summary>
    [DataField]
    public float MaxPitch = 1.1f;

    /// <summary>
    /// The volume of the bark.
    /// </summary>
    [DataField]
    public float Volume = 1;

    /// <summary>
    /// How often to play a sound.
    /// </summary>
    [DataField]
    public float Frequency = 0.5f;

    /// <summary>
    /// Stop the currently playing sound before playing a new one.
    /// </summary>
    [DataField]
    public bool Stop = false;

    /// <summary>
    /// Makes the audio predictable via hashing.
    /// </summary>
    [DataField]
    public bool Predictable = true;

    /// <summary>
    /// Whether it is available for selection in the character editor.
    /// </summary>
    [DataField]
    public bool RoundStart { get; private set; } = true;
}
