using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GDServer {
    public class TimeUtil {
        /// <summary>
        /// linux和win下datetime获取时间不一样，需要用这个方法保持一致
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCstDateTime() {
            Instant now = SystemClock.Instance.GetCurrentInstant();
            var shanghaiZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            return now.InZone(shanghaiZone).ToDateTimeUnspecified();
        }
    }
}
