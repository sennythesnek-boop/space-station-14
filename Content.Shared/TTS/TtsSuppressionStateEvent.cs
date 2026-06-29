using Robust.Shared.Serialization;

namespace Content.Shared.TTS;

/// <summary>
/// Sent from a client to the server whenever its TTS preferences change, so the server can suppress
/// built-in sounds (announcement chime, speech blips) that the client's TTS would otherwise overlap.
/// </summary>
[Serializable, NetSerializable]
public sealed class TtsSuppressionStateEvent(bool enabled, bool reading, bool readAnnouncements) : EntityEventArgs
{
    /// <summary>The client's master TTS toggle (<c>tts.client_enabled</c>).</summary>
    public bool Enabled = enabled;

    /// <summary>Neural read-aloud mode (<c>tts.reading</c>). Gibberish mode does not overlap SFX.</summary>
    public bool Reading = reading;

    /// <summary>Whether the client voices announcements (<c>tts.read_announcements</c>).</summary>
    public bool ReadAnnouncements = readAnnouncements;
}
