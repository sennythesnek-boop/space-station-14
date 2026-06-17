using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>Which standard vote a checkbox toggles.</summary>
public enum VoteToggle : byte
{
    Enabled,
    Restart,
    Preset,
    Map,
    Votekick,
}

/// <summary>A selectable map or gamemode row: its id, display name, and whether it's in the active profile.</summary>
[Serializable, NetSerializable]
public struct VoteConfigItem(string id, string display, bool included)
{
    public string Id = id;
    public string Display = display;
    public bool Included = included;
}

/// <summary>State for the voting config admin EUI (<c>votingconfig</c>).</summary>
[Serializable, NetSerializable]
public sealed class VoteConfigEuiState(
    bool canEdit,
    bool voteEnabled,
    bool restartVote,
    bool presetVote,
    bool mapVote,
    bool votekickVote,
    bool filterMapsByPlayerCount,
    List<string> mapProfiles,
    string activeMapProfile,
    List<VoteConfigItem> maps,
    List<string> presetProfiles,
    string activePresetProfile,
    List<VoteConfigItem> presets)
    : EuiStateBase
{
    public readonly bool CanEdit = canEdit;

    public readonly bool VoteEnabled = voteEnabled;
    public readonly bool RestartVote = restartVote;
    public readonly bool PresetVote = presetVote;
    public readonly bool MapVote = mapVote;
    public readonly bool VotekickVote = votekickVote;

    public readonly bool FilterMapsByPlayerCount = filterMapsByPlayerCount;

    public readonly List<string> MapProfiles = mapProfiles;
    public readonly string ActiveMapProfile = activeMapProfile;
    public readonly List<VoteConfigItem> Maps = maps;

    public readonly List<string> PresetProfiles = presetProfiles;
    public readonly string ActivePresetProfile = activePresetProfile;
    public readonly List<VoteConfigItem> Presets = presets;
}

// ---- Client -> server messages ----

[Serializable, NetSerializable]
public sealed class VoteConfigSetToggleMessage(VoteToggle toggle, bool value) : EuiMessageBase
{
    public readonly VoteToggle Toggle = toggle;
    public readonly bool Value = value;
}

[Serializable, NetSerializable]
public sealed class VoteConfigSetMapPlayerCountFilterMessage(bool value) : EuiMessageBase
{
    public readonly bool Value = value;
}

[Serializable, NetSerializable]
public sealed class VoteConfigSetActiveProfileMessage(bool isMap, string profile) : EuiMessageBase
{
    public readonly bool IsMap = isMap;
    public readonly string Profile = profile;
}

[Serializable, NetSerializable]
public sealed class VoteConfigCreateProfileMessage(bool isMap, string name) : EuiMessageBase
{
    public readonly bool IsMap = isMap;
    public readonly string Name = name;
}

[Serializable, NetSerializable]
public sealed class VoteConfigDeleteProfileMessage(bool isMap) : EuiMessageBase
{
    public readonly bool IsMap = isMap;
}

[Serializable, NetSerializable]
public sealed class VoteConfigSetItemMessage(bool isMap, string itemId, bool included) : EuiMessageBase
{
    public readonly bool IsMap = isMap;
    public readonly string ItemId = itemId;
    public readonly bool Included = included;
}
