namespace Content.Shared.Barks;

/// <summary>
/// Raised locally on the client to preview a bark (gibberish) voice in the character editor.
/// </summary>
public sealed class PreviewBarkEvent(string barkProtoId) : EntityEventArgs
{
    public string BarkProtoId { get; } = barkProtoId;
}
