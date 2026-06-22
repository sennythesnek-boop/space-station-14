using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Server.Administration;

/// <summary>
/// Admin-editable, persistent configuration for the "New Life" rule: the master settings (enabled, cooldown,
/// per-round cap — all CVar-backed) plus named profiles of event/game-rule IDs that block respawning while active.
/// Also tracks per-round, per-player new-life usage (in memory; not persisted).
/// Persisted as JSON to the server user-data dir, mirroring <see cref="VoteConfigManager"/>.
/// </summary>
public sealed partial class NewLifeManager
{
    [Dependency] private IResourceManager _res = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private static readonly ResPath Dir = new("/new_life_config");
    private static readonly ResPath ConfigFile = new("/new_life_config/config.json");
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    private ISawmill _sawmill = default!;

    // Whether the admin window has ever overridden the CVar-backed settings (so we re-apply on boot).
    private bool _settingsPersisted;

    private string _activeProfile = "";
    private readonly Dictionary<string, List<string>> _profiles = new();

    // Per-round usage, keyed by player. Cleared on round restart; never persisted.
    private readonly Dictionary<NetUserId, int> _used = new();

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("new_life");
        Load();
    }

    /// <summary>Re-applies persisted setting overrides after the config system is up.</summary>
    public void PostInitialize()
    {
        if (!_settingsPersisted)
            return;

        // The persisted values already live in the loaded CVar state via SetCVar at edit time; nothing extra to do
        // unless we stored them separately. We store them in the JSON and re-apply here to survive a fresh boot.
        _cfg.SetCVar(CCVars.NewLifeEnabled, _enabledStore);
        _cfg.SetCVar(CCVars.NewLifeCooldown, _cooldownStore);
        _cfg.SetCVar(CCVars.NewLifeMax, _maxStore);
        _cfg.SetCVar(CCVars.NewLifeKeepAntag, _keepAntagStore);
    }

    #region Settings (CVar-backed)

    // Shadow copies of the settings, kept in sync so they can be persisted to JSON and re-applied on boot.
    private bool _enabledStore;
    private float _cooldownStore = 300f;
    private int _maxStore = 3;
    private bool _keepAntagStore;

    public bool Enabled => _cfg.GetCVar(CCVars.NewLifeEnabled);
    public float Cooldown => _cfg.GetCVar(CCVars.NewLifeCooldown);
    public int MaxLives => _cfg.GetCVar(CCVars.NewLifeMax);
    public bool KeepAntag => _cfg.GetCVar(CCVars.NewLifeKeepAntag);

    public void SetEnabled(bool value)
    {
        _cfg.SetCVar(CCVars.NewLifeEnabled, value);
        _enabledStore = value;
        MarkPersistedAndSave();
    }

    public void SetCooldown(float value)
    {
        value = Math.Max(0f, value);
        _cfg.SetCVar(CCVars.NewLifeCooldown, value);
        _cooldownStore = value;
        MarkPersistedAndSave();
    }

    public void SetMax(int value)
    {
        value = Math.Max(0, value);
        _cfg.SetCVar(CCVars.NewLifeMax, value);
        _maxStore = value;
        MarkPersistedAndSave();
    }

    public void SetKeepAntag(bool value)
    {
        _cfg.SetCVar(CCVars.NewLifeKeepAntag, value);
        _keepAntagStore = value;
        MarkPersistedAndSave();
    }

    private void MarkPersistedAndSave()
    {
        _settingsPersisted = true;
        Save();
    }

    #endregion

    #region Profiles / blocklist

    public IReadOnlyList<string> ProfileNames => _profiles.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
    public string ActiveProfile => _activeProfile;

    public bool IsEventBlocked(string eventId)
    {
        return _activeProfile.Length != 0
            && _profiles.TryGetValue(_activeProfile, out var list)
            && list.Contains(eventId);
    }

    /// <summary>The set of event/game-rule IDs that currently block new lives (empty when no profile is active).</summary>
    public IReadOnlySet<string> GetBlockedSet()
    {
        if (_activeProfile.Length != 0 && _profiles.TryGetValue(_activeProfile, out var list))
            return list.ToHashSet();

        return new HashSet<string>();
    }

    public void SetActiveProfile(string profile)
    {
        // Empty string means "(none)" — nothing blocked.
        _activeProfile = profile.Length != 0 && _profiles.ContainsKey(profile) ? profile : "";
        Save();
    }

    public void CreateProfile(string name)
    {
        var safe = Sanitize(name);
        if (safe.Length == 0)
            return;

        if (!_profiles.ContainsKey(safe))
            _profiles[safe] = new List<string>();

        _activeProfile = safe;
        Save();
    }

    public void DeleteProfile()
    {
        if (_activeProfile.Length == 0)
            return;

        _profiles.Remove(_activeProfile);
        _activeProfile = "";
        Save();
    }

    public void SetEvent(string eventId, bool blocked)
    {
        if (_activeProfile.Length == 0 || !_profiles.TryGetValue(_activeProfile, out var list))
            return;

        if (blocked)
        {
            if (!list.Contains(eventId))
                list.Add(eventId);
        }
        else
        {
            list.Remove(eventId);
        }

        Save();
    }

    #endregion

    #region Per-round usage

    public int GetUsed(NetUserId user) => _used.GetValueOrDefault(user, 0);

    public void IncrementUsed(NetUserId user)
    {
        _used[user] = GetUsed(user) + 1;
    }

    /// <summary>Clears per-round usage. Called on round restart.</summary>
    public void ResetRound()
    {
        _used.Clear();
    }

    #endregion

    #region Persistence

    private void Save()
    {
        try
        {
            _res.UserData.CreateDir(Dir);
            using var writer = _res.UserData.OpenWriteText(ConfigFile);
            writer.Write(JsonSerializer.Serialize(BuildData(), JsonOpts));
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to write new life config: {e}");
        }
    }

    private void Load()
    {
        if (!_res.UserData.Exists(ConfigFile))
            return;

        try
        {
            using var reader = _res.UserData.OpenText(ConfigFile);
            var data = JsonSerializer.Deserialize<ConfigData>(reader.ReadToEnd(), JsonOpts);
            if (data == null)
                return;

            _settingsPersisted = data.SettingsPersisted;
            _enabledStore = data.Enabled;
            _cooldownStore = data.Cooldown;
            _maxStore = data.MaxLives;
            _keepAntagStore = data.KeepAntag;

            _activeProfile = data.ActiveProfile;
            _profiles.Clear();
            foreach (var (k, v) in data.Profiles)
                _profiles[k] = v;
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to read new life config: {e}");
        }
    }

    private ConfigData BuildData()
    {
        return new ConfigData
        {
            SettingsPersisted = _settingsPersisted,
            Enabled = _enabledStore,
            Cooldown = _cooldownStore,
            MaxLives = _maxStore,
            KeepAntag = _keepAntagStore,
            ActiveProfile = _activeProfile,
            Profiles = _profiles.ToDictionary(kv => kv.Key, kv => kv.Value),
        };
    }

    private static string Sanitize(string name)
    {
        var sb = new StringBuilder(name.Length);
        foreach (var c in name.Trim())
        {
            if (char.IsLetterOrDigit(c) || c is '-' or '_' or ' ')
                sb.Append(c);
        }

        return sb.ToString();
    }

    private sealed class ConfigData
    {
        public bool SettingsPersisted { get; set; }
        public bool Enabled { get; set; }
        public float Cooldown { get; set; } = 300f;
        public int MaxLives { get; set; } = 3;
        public bool KeepAntag { get; set; }
        public string ActiveProfile { get; set; } = "";
        public Dictionary<string, List<string>> Profiles { get; set; } = new();
    }

    #endregion
}
