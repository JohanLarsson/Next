using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Next
{
    public static class Extensions
    {
        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0) + TimeSpan.FromMilliseconds(unixTimeStamp);
            return new DateTime(dateTime.Ticks,DateTimeKind.Utc).ToLocalTime();
        }

        public static string ToBase64(this string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        public static T Deserialize<T>(this string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new XmlTextReader(new StringReader(xml)))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}