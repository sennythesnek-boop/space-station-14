using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>
/// State for the player records browser admin EUI (<c>playerrecords</c> command): one page of every user
/// who ever connected, newest seen first, with the key info an admin needs to find a returning player.
/// </summary>
[Serializable, NetSerializable]
public sealed class PlayerRecordsEuiState(
    string filter,
    int page,
    bool hasMore,
    List<PlayerRecordEntry> rows)
    : EuiStateBase
{
    public readonly string Filter = filter;
    public readonly int Page = page;

    /// <summary>Whether a further page exists after this one (enables the "Next" button).</summary>
    public readonly bool HasMore = hasMore;

    public readonly List<PlayerRecordEntry> Rows = rows;
}

/// <summary>
/// A single player row. Dates and address are pre-formatted server-side so the client just displays them.
/// </summary>
[Serializable, NetSerializable]
public struct PlayerRecordEntry(
    NetUserId userId,
    string userName,
    string firstSeen,
    string lastSeen,
    string address,
    TimeSpan playtime,
    int banCount,
    string? migratedFrom,
    string? migratedTo)
{
    public NetUserId UserId = userId;
    public string UserName = userName;
    public string FirstSeen = firstSeen;
    public string LastSeen = lastSeen;
    public string Address = address;
    public TimeSpan Playtime = playtime;
    public int BanCount = banCount;

    /// <summary>If set, this GUID received a completed migration from this username (the source).</summary>
    public string? MigratedFrom = migratedFrom;

    /// <summary>If set, this GUID's data was moved out to this username (the target).</summary>
    public string? MigratedTo = migratedTo;
}

/// <summary>
/// Sent from the client to (re)load a page of records for the given filter.
/// </summary>
[Serializable, NetSerializable]
public sealed class PlayerRecordsRequestMessage(string filter, int page) : EuiMessageBase
{
    public readonly string Filter = filter;
    public readonly int Page = page;
}
