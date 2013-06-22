using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;

namespace NextTests
{
    public class ExtensionsTests
    {
        private static readonly DateTime _dateTime = new DateTime(2013, 06, 20);
        private const long _unixTimeStamp = 1371679200000;

        [Test]
        public void ToUnixTimeStampTest()
        {
            long unixTimeStamp = _dateTime.ToUnixTimeStamp();
            Assert.AreEqual(_unixTimeStamp, unixTimeStamp);
        }

        [Test]
        public void ToDateTimeTest()
        {
            Assert.AreEqual(_dateTime, _unixTimeStamp.ToDateTime());
        }
    }
}
