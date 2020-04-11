using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public static class TimeHelper
    {
        public static TimeSpan ToTimeSpan(this DateTime time)
        {
            DateTime initTime = new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Utc);
            if (time.Kind != DateTimeKind.Utc)
            {
                time = time.ToUniversalTime();
            }

            return time - initTime;
        }

        public static DateTime ToDateTime(this TimeSpan timespan)
        {
            DateTime initTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return initTime.Add(timespan);
        }
        
        public static DateTime ToDateTime(this long milliseconds)
        {
            DateTime initTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return initTime.AddMilliseconds(milliseconds);
        }
    }
}
