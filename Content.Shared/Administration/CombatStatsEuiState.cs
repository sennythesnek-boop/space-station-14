using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>
/// State for the combat stats admin EUI (<c>combatstats</c> command). Per-player kills/damage/deaths
/// for the current round.
/// </summary>
[Serializable, NetSerializable]
public sealed class CombatStatsEuiState(
    int currentRound,
    int selectedRound,
    List<int> availableRounds,
    List<CombatStatEntry> entries) : EuiStateBase
{
    /// <summary>The round currently in progress (its stats are live).</summary>
    public readonly int CurrentRound = currentRound;

    /// <summary>The round whose stats are in <see cref="Entries"/>.</summary>
    public readonly int SelectedRound = selectedRound;

    /// <summary>Rounds that have stats available (saved past rounds + the current round), newest first.</summary>
    public readonly List<int> AvailableRounds = availableRounds;

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

/// <summary>Sent from the client to ask the server to rebuild the combat stats (re-enumerates rounds).</summary>
[Serializable, NetSerializable]
public sealed class CombatStatsRefreshMessage : EuiMessageBase;

/// <summary>Sent from the client to view a specific round's combat stats.</summary>
[Serializable, NetSerializable]
public sealed class CombatStatsSelectRoundMessage(int roundId) : EuiMessageBase
{
    public readonly int RoundId = roundId;
}
