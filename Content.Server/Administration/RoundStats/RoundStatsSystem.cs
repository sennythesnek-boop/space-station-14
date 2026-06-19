using Content.Shared.Administration;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Mobs;
using Robust.Shared.Player;
using Robust.Shared.Network;

namespace Content.Server.Administration.RoundStats;

/// <summary>
/// Tracks per-player combat statistics (kills, damage dealt, deaths) for the current round, in memory.
/// Resets on round restart. Surfaced read-only by the <c>combatstats</c> admin EUI.
/// </summary>
/// <remarks>
/// Standalone (does not use <c>KillTrackerComponent</c>/<c>KillReportedEvent</c>, which only exist for the
/// DeathMatch game mode). Damage is attributed via <see cref="DamageChangedEvent.Origin"/>; kills are
/// attributed on death to the finishing-blow origin, falling back to the largest cumulative damage source.
/// </remarks>
public sealed class RoundStatsSystem : EntitySystem
{
    private readonly Dictionary<NetUserId, PlayerCombatStats> _stats = new();

    /// <summary>Damage dealt to each victim by each player during that victim's current life (kill attribution).</summary>
    private readonly Dictionary<EntityUid, Dictionary<NetUserId, FixedPoint2>> _damageToVictim = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<DamageableComponent, DamageChangedEvent>(OnDamageChanged);
        // Broadcast form: the directed (MobStateComponent, MobStateChangedEvent) pair is already
        // claimed by the mob-state systems, and Robust allows only one directed sub per (comp, event).
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
    }

    private void OnRoundRestart(RoundRestartCleanupEvent ev)
    {
        _stats.Clear();
        _damageToVictim.Clear();
    }

    private PlayerCombatStats Get(NetUserId id)
    {
        if (!_stats.TryGetValue(id, out var s))
            _stats[id] = s = new PlayerCombatStats();
        return s;
    }

    private void OnDamageChanged(EntityUid uid, DamageableComponent component, DamageChangedEvent args)
    {
        if (!args.DamageIncreased || args.DamageDelta == null)
            return;

        // Only attribute damage that originated from a player.
        if (!TryComp<ActorComponent>(args.Origin, out var actor))
            return;

        // Ignore self-inflicted damage.
        if (args.Origin == uid)
            return;

        var amount = args.DamageDelta.GetTotal();
        if (amount <= 0)
            return;

        var attackerId = actor.PlayerSession.UserId;
        var stats = Get(attackerId);
        stats.LastOocName = actor.PlayerSession.Name;
        stats.LastCharacterName = MetaData(args.Origin!.Value).EntityName;
        stats.DamageTotal += amount;

        if (HasComp<ActorComponent>(uid))
            stats.DamagePlayers += amount;

        // Record cumulative damage to this victim for later kill attribution.
        if (!_damageToVictim.TryGetValue(uid, out var sources))
            _damageToVictim[uid] = sources = new Dictionary<NetUserId, FixedPoint2>();
        sources[attackerId] = sources.GetValueOrDefault(attackerId) + amount;
    }

    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        // Count a "kill" only on the transition into Dead.
        if (args.NewMobState != MobState.Dead || args.OldMobState == MobState.Dead)
            return;

        var victim = args.Target;
        var victimIsPlayer = TryComp<ActorComponent>(victim, out var victimActor);
        var victimId = victimActor?.PlayerSession.UserId;

        // Killer = finishing-blow origin if a player, otherwise the largest cumulative damage source.
        NetUserId? killerId = null;
        if (TryComp<ActorComponent>(args.Origin, out var originActor))
            killerId = originActor.PlayerSession.UserId;
        else if (_damageToVictim.TryGetValue(victim, out var sources))
            killerId = GetLargestSource(sources);

        // Don't count a suicide as a kill.
        if (killerId != null && !(victimIsPlayer && killerId == victimId))
        {
            var killer = Get(killerId.Value);
            killer.KillsTotal++;
            if (victimIsPlayer)
                killer.KillsPlayers++;
        }

        if (victimIsPlayer && victimActor != null)
        {
            var victimStats = Get(victimActor.PlayerSession.UserId);
            victimStats.LastOocName = victimActor.PlayerSession.Name;
            victimStats.LastCharacterName = MetaData(victim).EntityName;
            victimStats.Deaths++;
        }

        _damageToVictim.Remove(victim);
    }

    private static NetUserId? GetLargestSource(Dictionary<NetUserId, FixedPoint2> sources)
    {
        NetUserId? max = null;
        var maxDamage = FixedPoint2.Zero;
        foreach (var (id, dmg) in sources)
        {
            if (dmg < maxDamage)
                continue;
            max = id;
            maxDamage = dmg;
        }

        return max;
    }

    /// <summary>Builds a transport-safe snapshot of the current stats (FixedPoint2 → float).</summary>
    public List<CombatStatEntry> BuildEntries()
    {
        var result = new List<CombatStatEntry>(_stats.Count);
        foreach (var (_, s) in _stats)
        {
            var character = string.IsNullOrEmpty(s.LastCharacterName) ? s.LastOocName : s.LastCharacterName;
            result.Add(new CombatStatEntry(
                s.LastOocName,
                character,
                s.KillsPlayers,
                s.KillsTotal,
                (float) s.DamagePlayers,
                (float) s.DamageTotal,
                s.Deaths));
        }

        return result;
    }

    private sealed class PlayerCombatStats
    {
        public string LastOocName = string.Empty;
        public string LastCharacterName = string.Empty;
        public int KillsPlayers;
        public int KillsTotal;
        public FixedPoint2 DamagePlayers;
        public FixedPoint2 DamageTotal;
        public int Deaths;
    }
}
