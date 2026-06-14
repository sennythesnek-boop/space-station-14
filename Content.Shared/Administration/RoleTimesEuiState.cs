using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>
/// State for the role time overview admin EUI (<c>roletimes</c> command).
/// Sent from the server with the full set of trackers (roles) and their accumulated time.
/// </summary>
[Serializable, NetSerializable]
public sealed class RoleTimesEuiState(
    NetUserId userId,
    string username,
    bool online,
    bool canEdit,
    List<RoleTimeInfo> roles)
    : EuiStateBase
{
    public readonly NetUserId UserId = userId;
    public readonly string Username = username;

    /// <summary>Whether the target player is currently connected. Affects how edits are applied.</summary>
    public readonly bool Online = online;

    /// <summary>Whether the viewing admin is allowed to change times.</summary>
    public readonly bool CanEdit = canEdit;

    public readonly List<RoleTimeInfo> Roles = roles;
}

/// <summary>
/// A single tracker row: the raw tracker id, a friendly display name, the accumulated time, and an
/// optional human-readable summary of the play time requirements gating the associated job.
/// </summary>
[Serializable, NetSerializable]
public struct RoleTimeInfo(string tracker, string displayName, TimeSpan time, string? requirement)
{
    public string Tracker = tracker;
    public string DisplayName = displayName;
    public TimeSpan Time = time;

    /// <summary>Pre-formatted "Requires: ..." text, or null if the role has no time requirements.</summary>
    public string? Requirement = requirement;
}

/// <summary>
/// Sent from the client to set a tracker to an absolute time value.
/// </summary>
[Serializable, NetSerializable]
public sealed class RoleTimesSetMessage(string tracker, TimeSpan time) : EuiMessageBase
{
    public readonly string Tracker = tracker;
    public readonly TimeSpan Time = time;
}
