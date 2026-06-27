using System;

namespace Content.Shared.Database;

/// <summary>
/// Lifecycle state of a user-data migration recorded in the <c>migration_log</c> table.
/// </summary>
public enum MigrationStatus
{
    /// <summary>
    /// A same-username re-registration was detected but could not be auto-confirmed (no HWID/IP
    /// overlap). No data has been moved; an admin must approve or reject it in the migrations tool.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The data was migrated (either automatically after an overlap match, or by an admin).
    /// </summary>
    Completed = 1,

    /// <summary>
    /// An admin reviewed a pending migration and declined it. No data was moved.
    /// </summary>
    Rejected = 2,
}

/// <summary>
/// Which groups of per-user data a migration should move. Combined as flags so the automatic path
/// can move a safe subset while the manual tool may opt into everything (including admin status).
/// </summary>
[Flags]
public enum MigrationScope
{
    None = 0,

    /// <summary>Play/role times, preferences and characters. The core "my progress is gone" fix.</summary>
    Gameplay = 1 << 0,

    /// <summary>Server whitelist, blacklist and per-job role whitelists.</summary>
    Whitelists = 1 << 1,

    /// <summary>Bans, ban exemptions, admin notes/watchlists/messages. Prevents ban-evasion via re-register.</summary>
    Bans = 1 << 2,

    /// <summary>
    /// Admin rank and flags. DANGEROUS on a mere username match — never set by the automatic path,
    /// only ever by an explicit manual transfer.
    /// </summary>
    Admin = 1 << 3,

    /// <summary>Everything the automatic path is allowed to move (no admin status).</summary>
    Auto = Gameplay | Whitelists | Bans,

    /// <summary>Every group, including admin status. Manual transfers only.</summary>
    All = Gameplay | Whitelists | Bans | Admin,
}
