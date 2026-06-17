using System.Linq;
using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Server.GameTicking.Presets;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration;

/// <summary>Server side of the voting config admin window. Edits the <see cref="VoteConfigManager"/>.</summary>
public sealed partial class VoteConfigEui : BaseEui
{
    [Dependency] private IAdminManager _admins = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private VoteConfigManager _config = default!;

    public VoteConfigEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override void Opened()
    {
        base.Opened();
        _admins.OnPermsChanged += OnPermsChanged;
    }

    public override void Closed()
    {
        base.Closed();
        _admins.OnPermsChanged -= OnPermsChanged;
    }

    private void OnPermsChanged(AdminPermsChangedEventArgs args)
    {
        if (args.Player == Player)
            BuildState();
    }

    private bool CanView() => _admins.HasAdminFlag(Player, AdminFlags.Admin);
    private bool CanEdit() => _admins.HasAdminFlag(Player, AdminFlags.Server);

    public override EuiStateBase GetNewState() => _state;

    private VoteConfigEuiState _state = new(false, true, true, true, false, true,
        new(), "", new(), new(), "", new());

    public void BuildState()
    {
        if (!CanView())
        {
            Close();
            return;
        }

        var maps = new List<VoteConfigItem>();
        foreach (var map in _proto.EnumeratePrototypes<GameMapPrototype>())
            maps.Add(new VoteConfigItem(map.ID, Loc.GetString(map.MapName), _config.IsItemIncluded(true, map.ID)));
        maps.Sort((a, b) => string.Compare(a.Display, b.Display, StringComparison.OrdinalIgnoreCase));

        var presets = new List<VoteConfigItem>();
        foreach (var preset in _proto.EnumeratePrototypes<GamePresetPrototype>())
            presets.Add(new VoteConfigItem(preset.ID, Loc.GetString(preset.ModeTitle), _config.IsItemIncluded(false, preset.ID)));
        presets.Sort((a, b) => string.Compare(a.Display, b.Display, StringComparison.OrdinalIgnoreCase));

        _state = new VoteConfigEuiState(
            CanEdit(),
            _config.GetToggle(VoteToggle.Enabled),
            _config.GetToggle(VoteToggle.Restart),
            _config.GetToggle(VoteToggle.Preset),
            _config.GetToggle(VoteToggle.Map),
            _config.GetToggle(VoteToggle.Votekick),
            _config.MapProfileNames.ToList(),
            _config.ActiveMapProfile,
            maps,
            _config.PresetProfileNames.ToList(),
            _config.ActivePresetProfile,
            presets);

        StateDirty();
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (!CanEdit())
            return;

        switch (msg)
        {
            case VoteConfigSetToggleMessage m:
                _config.SetToggle(m.Toggle, m.Value);
                break;
            case VoteConfigSetActiveProfileMessage m:
                _config.SetActiveProfile(m.IsMap, m.Profile);
                break;
            case VoteConfigCreateProfileMessage m:
                _config.CreateProfile(m.IsMap, m.Name);
                break;
            case VoteConfigDeleteProfileMessage m:
                _config.DeleteProfile(m.IsMap);
                break;
            case VoteConfigSetItemMessage m:
                _config.SetItem(m.IsMap, m.ItemId, m.Included);
                break;
            default:
                return;
        }

        BuildState();
    }
}
