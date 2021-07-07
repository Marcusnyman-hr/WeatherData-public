using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherData.Web.Helpers
{
    public static class DateTimeConverter
    {
        /// <summary>
        /// DateTime converter 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
      public static long MillisecondsTimestamp(this DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(date.ToUniversalTime() - baseDate).TotalMilliseconds;
        }
    }
}
