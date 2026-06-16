namespace Content.Shared.Chat;

/// <summary>
/// Helper for the job-icon markup used to show a speaker's job icon before their name in radio.
/// </summary>
public static class ChatIconTokens
{
    /// <summary>Builds the job-icon markup for a player's job, to prepend before their name in radio.</summary>
    public static string JobIconMarkup(string jobId)
    {
        // Markup attribute values must be quoted (the parser only accepts quoted strings/numbers/colors).
        return $"[chaticon kind=\"job\" key=\"{jobId}\"]";
    }
}
