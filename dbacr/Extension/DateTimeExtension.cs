namespace Dbacr.Extension;

public static class DateTimeExtension
{
    public static string? GetTimeString(this DateTime time)
    {
        var date =  time - DateTime.Now;            

        if (date.Days > 0)
            return $"{date.Days:N0} {"Day".Pluralize(date.Days)}";
        if (date.Days == 0 && date.Hours > 0)
            return $"{date.Hours:N0} {"Hour".Pluralize(date.Hours)}";
        if (date.Days == 0 && date.Hours == 0 && date.Minutes > 0)
            return $"{date.Minutes:N0} {"Minute".Pluralize(date.Minutes)}";
        if (date.Days == 0 && date.Hours == 0 && date.Minutes > 0 && date.Seconds > 0)
            return $"{date.Seconds:N0} {"Second".Pluralize(date.Seconds)}";
        if (date.Days == 0 && date.Hours == 0 && date.Minutes == 0 && date.Seconds == 0 && date.Milliseconds > 0)
            return $"{date.Milliseconds:N0} {"Millisecond".Pluralize(date.Milliseconds)}";

        return default;
    }

    public static string? GetTimeString(this DateTimeOffset time)
    {
        var date = DateTime.Now - time;

        if (date.Days > 0)
            return $"{date.Days:N0} {"Day".Pluralize(date.Days)}";
        if (date.Days == 0 && date.Hours > 0)
            return $"{date.Hours:N0} {"Hour".Pluralize(date.Hours)}";
        if (date.Days == 0 && date.Hours == 0 && date.Minutes > 0)
            return $"{date.Minutes:N0} {"Minute".Pluralize(date.Minutes)}";
        if (date.Days == 0 && date.Hours == 0 && date.Minutes > 0 && date.Seconds > 0)
            return $"{date.Seconds:N0} {"Second".Pluralize(date.Seconds)}";
        if (date.Days == 0 && date.Hours == 0 && date.Minutes == 0 && date.Seconds == 0 && date.Milliseconds > 0)
            return $"{date.Milliseconds:N0} {"Millisecond".Pluralize(date.Milliseconds)}";

        return default;
    }
}
