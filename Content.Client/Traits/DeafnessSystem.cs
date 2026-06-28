using Content.Shared.Traits.Assorted;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client.Traits;

/// <summary>
/// Mutes all audio for the local player while they have a <see cref="DeafComponent"/>.
/// </summary>
public sealed partial class DeafnessSystem : EntitySystem
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private IAudioManager _audio = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private float _originalVolume;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DeafComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<DeafComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<DeafComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DeafComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
    }

    private void Mute()
    {
        // Save the current volume so it can be restored when deafness ends.
        _originalVolume = _cfg.GetCVar(CVars.AudioMasterVolume);
        _audio.SetMasterGain(0);
    }

    private void Unmute()
    {
        _audio.SetMasterGain(_originalVolume);
    }

    private void OnStartup(EntityUid uid, DeafComponent component, ComponentStartup args)
    {
        if (_player.LocalSession?.AttachedEntity == uid)
            Mute();
    }

    private void OnShutdown(EntityUid uid, DeafComponent component, ComponentShutdown args)
    {
        if (_player.LocalSession?.AttachedEntity == uid)
            Unmute();
    }

    private void OnPlayerAttached(EntityUid uid, DeafComponent component, LocalPlayerAttachedEvent args)
    {
        Mute();
    }

    private void OnPlayerDetached(EntityUid uid, DeafComponent component, LocalPlayerDetachedEvent args)
    {
        Unmute();
    }
}
