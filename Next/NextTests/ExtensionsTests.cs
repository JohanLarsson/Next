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
        [Test]
        public void ToUnixTimeStampTest()
        {
            DateTime dateTime = new DateTime(2013, 06, 20);
            long unixTimeStamp = dateTime.ToUnixTimeStamp();
            Assert.AreEqual(1371679200000, unixTimeStamp);
            DateTime time = unixTimeStamp.ToDateTime();
            Assert.AreEqual(dateTime, time);
        }
    }
}
