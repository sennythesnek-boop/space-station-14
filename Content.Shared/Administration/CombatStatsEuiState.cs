using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>
/// State for the combat stats admin EUI (<c>combatstats</c> command). Per-player kills/damage/deaths
/// for the current round.
/// </summary>
[Serializable, NetSerializable]
public sealed class CombatStatsEuiState(List<CombatStatEntry> entries) : EuiStateBase
{
    public readonly List<CombatStatEntry> Entries = entries;
}

/// <summary>
/// One player's combat stats. "Players" columns count only player-vs-player; "total" columns count all combat.
/// </summary>
[Serializable, NetSerializable]
public struct CombatStatEntry(
    string oocName,
    string characterName,
    int killsPlayers,
    int killsTotal,
    float damagePlayers,
    float damageTotal,
    int deaths)
{
    public string OocName = oocName;
    public string CharacterName = characterName;
    public int KillsPlayers = killsPlayers;
    public int KillsTotal = killsTotal;
    public float DamagePlayers = damagePlayers;
    public float DamageTotal = damageTotal;
    public int Deaths = deaths;
}

/// <summary>Sent from the client to ask the server to rebuild the combat stats.</summary>
[Serializable, NetSerializable]
public sealed class CombatStatsRefreshMessage : EuiMessageBase;
