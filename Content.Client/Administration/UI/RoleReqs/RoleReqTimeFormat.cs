using System;
using System.Globalization;

namespace Content.Client.Administration.UI.RoleReqs;

/// <summary>Shared HH:MM / minutes parsing and formatting for the role requirement editor.</summary>
public static class RoleReqTimeFormat
{
    public static string Format(TimeSpan time)
    {
        return $"{(int) time.TotalHours:00}:{time.Minutes:00}";
    }

    public static bool TryParse(string text, out TimeSpan time)
    {
        time = default;
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
            return false;

        if (text.Contains(':'))
        {
            var parts = text.Split(':');
            if (parts.Length != 2
                || !int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var hours)
                || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var minutes)
                || hours < 0 || minutes < 0)
                return false;

            time = new TimeSpan(hours, minutes, 0);
            return true;
        }

        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var totalMinutes)
            || totalMinutes < 0)
            return false;

        time = TimeSpan.FromMinutes(totalMinutes);
        return true;
    }
}
