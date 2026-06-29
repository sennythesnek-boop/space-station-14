using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>
/// State for the migration oversight admin EUI (<c>migrations</c> command): the recent migration log plus
/// the result of the admin's last action. Guids/enums are pre-formatted server-side into display strings.
/// </summary>
[Serializable, NetSerializable]
public sealed class MigrationsEuiState(string? lastResult, bool canMigrateAdmin, List<MigrationRow> rows) : EuiStateBase
{
    /// <summary>Message describing the outcome of the admin's last action (transfer/approve/reject), if any.</summary>
    public readonly string? LastResult = lastResult;

    /// <summary>Whether this viewer may migrate admin status (full Admin flag). Gates the Admin scope checkbox.</summary>
    public readonly bool CanMigrateAdmin = canMigrateAdmin;

    public readonly List<MigrationRow> Rows = rows;
}

[Serializable, NetSerializable]
public struct MigrationRow(
    int id,
    string source,
    string target,
    string sourceName,
    string targetName,
    string time,
    bool automatic,
    bool pending,
    string statusText,
    string scopeText,
    string matchReason,
    string detail)
{
    public int Id = id;
    public string Source = source;
    public string Target = target;
    public string SourceName = sourceName;
    public string TargetName = targetName;
    public string Time = time;
    public bool Automatic = automatic;

    /// <summary>True while awaiting review; the client shows Approve/Reject for these.</summary>
    public bool Pending = pending;

    public string StatusText = statusText;
    public string ScopeText = scopeText;
    public string MatchReason = matchReason;
    public string Detail = detail;
}

/// <summary>Approve a pending (name-only) migration, performing the move.</summary>
[Serializable, NetSerializable]
public sealed class MigrationApproveMessage(int id) : EuiMessageBase
{
    public readonly int Id = id;
}

/// <summary>Reject a pending migration, leaving data untouched.</summary>
[Serializable, NetSerializable]
public sealed class MigrationRejectMessage(int id) : EuiMessageBase
{
    public readonly int Id = id;
}

/// <summary>
/// Manually transfer data from one player to another. Source/target are name-or-GUID strings resolved
/// server-side; scope is the combined <see cref="MigrationScope"/> flags as an int.
/// </summary>
[Serializable, NetSerializable]
public sealed class MigrationManualMessage(string source, string target, int scope, bool merge) : EuiMessageBase
{
    public readonly string Source = source;
    public readonly string Target = target;
    public readonly int Scope = scope;

    /// <summary>True to combine both accounts' data; false to replace the target's with the source's.</summary>
    public readonly bool Merge = merge;
}
