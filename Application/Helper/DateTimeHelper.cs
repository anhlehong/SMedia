using System;

public static class DateTimeHelper
{
    public static DateTime GetVietnamTime()
    {
        try
        {
            var vietnamTime = DateTime.UtcNow.AddHours(7);
            Console.WriteLine($"GetVietnamTime: {vietnamTime:yyyy-MM-dd HH:mm:ss} (+07:00)");
            return vietnamTime;
        }
        catch (Exception ex)
        {
            // Rất hiếm khi xảy ra, nhưng thêm để đảm bảo an toàn
            Console.WriteLine($"Error in GetVietnamTime: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return DateTime.UtcNow; // Fallback to UTC
        }
    }
}