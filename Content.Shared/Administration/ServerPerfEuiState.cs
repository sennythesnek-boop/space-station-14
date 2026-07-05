using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

/// <summary>
/// State for the server performance admin EUI (<c>serverperf</c> command).
/// One snapshot per second while the window is open.
/// </summary>
[Serializable, NetSerializable]
public sealed class ServerPerfEuiState : EuiStateBase
{
    /// <summary>The configured tick rate (net.tickrate).</summary>
    public int TargetTickRate;

    /// <summary>Ticks actually processed in the last second.</summary>
    public float ActualTps;

    /// <summary>The time budget for one tick in milliseconds (1000 / tickrate).</summary>
    public float TickBudgetMs;

    /// <summary>Average wall-clock spacing between ticks over the last second, in milliseconds.</summary>
    public float AvgTickMs;

    /// <summary>Worst tick spacing over the last second, in milliseconds.</summary>
    public float MaxTickMs;

    /// <summary>Ticks in the last minute that ran more than 25% over budget.</summary>
    public int LateTicksLastMinute;

    /// <summary>CPU cores' worth of processor time the server consumed in the last second (1.0 = one full core).</summary>
    public float CpuCoresUsed;

    /// <summary>Total logical processors on the host.</summary>
    public int ProcessorCount;

    public long WorkingSetBytes;
    public long ManagedHeapBytes;

    /// <summary>GC collections in the last minute, per generation.</summary>
    public int Gc0LastMinute;
    public int Gc1LastMinute;
    public int Gc2LastMinute;

    /// <summary>Percentage of runtime the process has spent paused for garbage collection.</summary>
    public float GcPausePercent;

    public int EntityCount;
    public int PlayerCount;

    public long NetSentBytesPerSec;
    public long NetRecvBytesPerSec;

    public TimeSpan Uptime;

    /// <summary>Per-second history, oldest first. All arrays have equal length.</summary>
    public float[] HistoryAvgTickMs = Array.Empty<float>();
    public float[] HistoryMaxTickMs = Array.Empty<float>();
    public float[] HistoryCpuCores = Array.Empty<float>();
}
