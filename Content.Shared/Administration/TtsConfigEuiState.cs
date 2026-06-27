using Content.Shared.Eui;
using Content.Shared.TTS;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>Which TTS config value a panel field edits.</summary>
public enum TtsConfigField : byte
{
    Enabled,
    ApiUrl,
    SpeakersUrl,
    ModelsUrl,
    ApiToken,
    DefaultSpeaker,
    Model,
    Speed,
    MaxLength,
    QueueDelay,
}

/// <summary>A backend voice, the language group it belongs to, and its configured gender bucket.</summary>
[Serializable, NetSerializable]
public struct TtsVoiceItem(string speaker, string language, TtsVoiceGender gender)
{
    public string Speaker = speaker;
    public string Language = language;
    public TtsVoiceGender Gender = gender;
}

/// <summary>State for the TTS config admin EUI (<c>ttsconfig</c>).</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigEuiState(
    bool canEdit,
    bool enabled,
    string apiUrl,
    string speakersUrl,
    string modelsUrl,
    string apiToken,
    string defaultSpeaker,
    string model,
    float speed,
    int maxLength,
    float queueDelay,
    List<string> models,
    List<TtsVoiceItem> voices,
    string status)
    : EuiStateBase
{
    public readonly bool CanEdit = canEdit;
    public readonly bool Enabled = enabled;
    public readonly string ApiUrl = apiUrl;
    public readonly string SpeakersUrl = speakersUrl;
    public readonly string ModelsUrl = modelsUrl;
    public readonly string ApiToken = apiToken;
    public readonly string DefaultSpeaker = defaultSpeaker;
    public readonly string Model = model;
    public readonly float Speed = speed;
    public readonly int MaxLength = maxLength;
    public readonly float QueueDelay = queueDelay;

    /// <summary>Available backend models.</summary>
    public readonly List<string> Models = models;

    /// <summary>Available backend voices and their configured genders.</summary>
    public readonly List<TtsVoiceItem> Voices = voices;

    /// <summary>Result of the last test/refresh action, shown in the panel.</summary>
    public readonly string Status = status;
}

// ---- Client -> server messages ----

/// <summary>Sets one scalar TTS config value. The server parses <see cref="Value"/> per <see cref="Field"/>.</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigSetMessage(TtsConfigField field, string value) : EuiMessageBase
{
    public readonly TtsConfigField Field = field;
    public readonly string Value = value;
}

/// <summary>Assigns a gender bucket to a backend voice.</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigSetVoiceGenderMessage(string speaker, TtsVoiceGender gender) : EuiMessageBase
{
    public readonly string Speaker = speaker;
    public readonly TtsVoiceGender Gender = gender;
}

/// <summary>Assigns the same gender bucket to every known voice (bulk convenience).</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigSetAllGenderMessage(TtsVoiceGender gender) : EuiMessageBase
{
    public readonly TtsVoiceGender Gender = gender;
}

/// <summary>Auto-assigns genders from the voice-name convention (Kokoro ids).</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigAutoGenderMessage : EuiMessageBase
{
}

/// <summary>Asks the server to (re)fetch the voice and model lists from the backend.</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigRefreshSpeakersMessage : EuiMessageBase
{
}

/// <summary>Asks the server to synthesize and play back a test line using the current settings.</summary>
[Serializable, NetSerializable]
public sealed class TtsConfigTestMessage(string text, string speaker) : EuiMessageBase
{
    public readonly string Text = text;
    public readonly string Speaker = speaker;
}
