namespace Content.Shared.TTS;

/// <summary>
/// Which character gender a neural backend voice is assigned to. <see cref="None"/> means the
/// voice is unavailable (never used). Characters whose gender isn't male/female (they/them, it/its)
/// draw a random voice from the whole assigned pool instead.
/// </summary>
public enum TtsVoiceGender : byte
{
    /// <summary>Voice is excluded from all pools.</summary>
    None = 0,
    Male = 1,
    Female = 2,
}
