using System;
namespace HodlWallet2.Core.Utils
{
    public class DateTimeOffsetOperations
    {
        public static string shortDate(DateTimeOffset dateTime)
        {
            var current = DateTimeOffset.Now - dateTime;
            if (current.TotalMinutes < 60)
                return (int)current.TotalMinutes + " m";
            else if (current.TotalHours < 24)
                return (int)current.TotalHours + " h";
            else
                return dateTime.Date.ToShortDateString();
        }
    }
}
