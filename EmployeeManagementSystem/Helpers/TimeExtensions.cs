using System;

namespace EmployeeManagementSystem.Helpers
{
    public static class TimeExtensions
    {
        // For nullable TimeSpan
        public static string ToTimeString(this TimeSpan? time)
        {
            return time.HasValue ? time.Value.ToString(@"hh\:mm") : "-";
        }

        // Optional: For non-nullable TimeSpan
        public static string ToTimeString(this TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
}
