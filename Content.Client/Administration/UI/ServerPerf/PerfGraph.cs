using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;

namespace Content.Client.Administration.UI.ServerPerf;

/// <summary>
/// Minimal line graph for the server performance window: one or more series
/// plus an optional horizontal reference line (e.g. the tick budget), auto-scaled.
/// </summary>
public sealed class PerfGraph : Control
{
    private static readonly Color BackgroundColor = Color.FromHex("#101216");
    private static readonly Color GridColor = Color.FromHex("#2a2e38");

    private readonly List<(float[] Data, Color Color)> _series = new();

    /// <summary>Value of the horizontal reference line. 0 disables it.</summary>
    public float ReferenceLine;

    public Color ReferenceColor = Color.FromHex("#c8a825");

    /// <summary>The y-axis covers at least this value even when all data is smaller.</summary>
    public float MinScale = 1f;

    public void SetSeries(params (float[] Data, Color Color)[] series)
    {
        _series.Clear();
        _series.AddRange(series);
        InvalidateMeasure();
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        var size = PixelSize;
        handle.DrawRect(new UIBox2(Vector2.Zero, size), BackgroundColor);

        var peak = Math.Max(MinScale, ReferenceLine * 1.5f);
        foreach (var (data, _) in _series)
        {
            foreach (var v in data)
            {
                if (v > peak)
                    peak = v;
            }
        }

        // A bit of headroom so peaks don't touch the top edge.
        peak *= 1.05f;

        // Horizontal quarter grid lines.
        for (var i = 1; i < 4; i++)
        {
            var y = size.Y * i / 4f;
            handle.DrawLine(new Vector2(0, y), new Vector2(size.X, y), GridColor);
        }

        if (ReferenceLine > 0)
        {
            var y = ToY(ReferenceLine, peak, size.Y);
            handle.DrawLine(new Vector2(0, y), new Vector2(size.X, y), ReferenceColor);
        }

        foreach (var (data, color) in _series)
        {
            if (data.Length < 2)
                continue;

            var stepX = size.X / (data.Length - 1f);
            var prev = new Vector2(0, ToY(data[0], peak, size.Y));
            for (var i = 1; i < data.Length; i++)
            {
                var point = new Vector2(stepX * i, ToY(data[i], peak, size.Y));
                handle.DrawLine(prev, point, color);
                prev = point;
            }
        }
    }

    private static float ToY(float value, float peak, float height)
    {
        return height - Math.Clamp(value / peak, 0f, 1f) * height;
    }
}
