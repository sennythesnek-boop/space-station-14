using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>A selectable event/game-rule row: its id, display name, and whether it's blocked in the active profile.</summary>
[Serializable, NetSerializable]
public struct NewLifeEventItem(string id, string display, bool blocked)
{
    public string Id = id;
    public string Display = display;
    public bool Blocked = blocked;
}

/// <summary>State for the New Life config admin EUI (<c>newlifeconfig</c>).</summary>
[Serializable, NetSerializable]
public sealed class NewLifeConfigEuiState(
    bool canEdit,
    bool enabled,
    float cooldown,
    int maxLives,
    bool keepAntag,
    List<string> profiles,
    string activeProfile,
    List<NewLifeEventItem> events,
    bool blockedNow,
    string blockedReason)
    : EuiStateBase
{
    public readonly bool CanEdit = canEdit;

    public readonly bool Enabled = enabled;
    public readonly float Cooldown = cooldown;
    public readonly int MaxLives = maxLives;
    public readonly bool KeepAntag = keepAntag;

    public readonly List<string> Profiles = profiles;
    public readonly string ActiveProfile = activeProfile;
    public readonly List<NewLifeEventItem> Events = events;

    /// <summary>Whether new lives are blocked right now, and a human-readable reason (active rule / evac / not in round).</summary>
    public readonly bool BlockedNow = blockedNow;
    public readonly string BlockedReason = blockedReason;
}

// ---- Client -> server messages ----

[Serializable, NetSerializable]
public sealed class NewLifeSetEnabledMessage(bool value) : EuiMessageBase
{
    public readonly bool Value = value;
}

[Serializable, NetSerializable]
public sealed class NewLifeSetCooldownMessage(float value) : EuiMessageBase
{
    public readonly float Value = value;
}

[Serializable, NetSerializable]
public sealed class NewLifeSetMaxMessage(int value) : EuiMessageBase
{
    public readonly int Value = value;
}

[Serializable, NetSerializable]
public sealed class NewLifeSetKeepAntagMessage(bool value) : EuiMessageBase
{
    public readonly bool Value = value;
}

[Serializable, NetSerializable]
public sealed class NewLifeSetActiveProfileMessage(string profile) : EuiMessageBase
{
    public readonly string Profile = profile;
}

[Serializable, NetSerializable]
public sealed class NewLifeCreateProfileMessage(string name) : EuiMessageBase
{
    public readonly string Name = name;
}

[Serializable, NetSerializable]
public sealed class NewLifeDeleteProfileMessage : EuiMessageBase
{
}

[Serializable, NetSerializable]
public sealed class NewLifeSetEventMessage(string eventId, bool blocked) : EuiMessageBase
{
    public readonly string EventId = eventId;
    public readonly bool Blocked = blocked;
}
