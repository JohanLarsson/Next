using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Next
{
    public static class Extensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();

        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - Epoch).TotalMilliseconds;
        }

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            DateTime dateTime = Epoch.AddMilliseconds(unixTimeStamp);
            return new DateTime(dateTime.Ticks,DateTimeKind.Utc).ToLocalTime();
        }

        public static string ToBase64(this string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

    }
}