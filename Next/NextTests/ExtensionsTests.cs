using System;

using NUnit.Framework;
using Next;

namespace NextTests
{
    public class ExtensionsTests
    {
        private static readonly DateTime _dateTime = new DateTime(2013, 06, 19, 23, 0, 0, 0, DateTimeKind.Utc);
        private const long _unixTimeStamp = 1371682800000; // June 20 2013 in milliseconds after unix epoch

        [Test]
        public void ToUnixTimeStampTest()
        {
            long unixTimeStamp = _dateTime.ToUnixTimeStamp();
            Assert.AreEqual(_unixTimeStamp, unixTimeStamp);
        }

        [Test]
        public void ToDateTimeTest()
        {
            Assert.AreEqual(_dateTime, _unixTimeStamp.ToDateTime().ToUniversalTime());
        }
    }
}
