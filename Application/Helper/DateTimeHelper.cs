using System;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public static DateTime GetVietnamTime()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
    }
}