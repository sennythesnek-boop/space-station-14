using Robust.Shared.Serialization;

namespace Content.Shared.TTS;

/// <summary>
/// Sent from a client to the server asking it to synthesize a line of neural speech.
/// The server resolves the voice, calls the configured TTS backend, and replies with either
/// a <see cref="PlayTtsEvent"/> (success) or a <see cref="TtsFallbackEvent"/> (backend
/// unavailable) so the client can fall back to gibberish.
/// </summary>
[Serializable, NetSerializable]
public sealed class RequestTtsEvent(string text, string voiceId, NetEntity source, bool whisper) : EntityEventArgs
{
    public string Text { get; } = text;

    /// <summary>The bark/voice prototype id; the server maps it to a backend speaker.</summary>
    public string VoiceId { get; } = voiceId;

    /// <summary>Entity to play the audio from, or <see cref="NetEntity.Invalid"/> for global playback.</summary>
    public NetEntity Source { get; } = source;

    /// <summary>Whether the line was whispered (only used for the gibberish fallback).</summary>
    public bool Whisper { get; } = whisper;
}

/// <summary>
/// Server reply carrying synthesized audio for the requesting client to play.
/// </summary>
[Serializable, NetSerializable]
public sealed class PlayTtsEvent(byte[] data, NetEntity source) : EntityEventArgs
{
    public byte[] Data { get; } = data;

    /// <summary>Entity to play from, or <see cref="NetEntity.Invalid"/> for global playback.</summary>
    public NetEntity Source { get; } = source;
}

/// <summary>
/// Server reply telling the client neural synthesis was unavailable (backend offline, not
/// configured, or errored). The client falls back to playing the line as gibberish so the
/// player still hears something.
/// </summary>
[Serializable, NetSerializable]
public sealed class TtsFallbackEvent(string text, string voiceId, NetEntity source, bool whisper) : EntityEventArgs
{
    public string Text { get; } = text;
    public string VoiceId { get; } = voiceId;
    public NetEntity Source { get; } = source;
    public bool Whisper { get; } = whisper;
}
