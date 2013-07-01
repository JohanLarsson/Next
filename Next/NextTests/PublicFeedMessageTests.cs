using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;
using NextTests.Helpers;

namespace NextTests
{
    public class PublicFeedMessageTests
    {
        [Test]
        public void PriceTickTest()
        {
            string priceTick = Properties.Settings.Default.PriceTickJson;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch deserializeSw = Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            stopwatch.Restart();

            Assert.IsTrue(PublicFeedMessage.IsPriceTick(priceTick));
            Console.WriteLine("PublicFeedMessage.IsPriceTick(priceTick) took: " + stopwatch.GetTimeString());
            deserializeSw.Restart();
            PriceTick tick = PublicFeedMessage.Deserialize(priceTick);
            Console.WriteLine("PublicFeedMessage.Deserialize(priceTick) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());
            Assert.AreEqual(new InstrumentDescriptor(30, "1843"), tick.Instrument);
            Assert.AreEqual(new DateTime(2010, 01, 22, 16, 17, 23), tick.trade_timestamp);
            Assert.AreEqual(new DateTime(2010, 01, 22, 16, 18, 42), tick.tick_timestamp);

            Assert.AreEqual(0, tick.bid);
            Assert.AreEqual(0, tick.bid_volume);
            Assert.AreEqual(101, tick.ask);
            Assert.AreEqual(200, tick.ask_volume);
            Assert.AreEqual(101, tick.close);
            Assert.AreEqual(101, tick.close);
            Assert.AreEqual(101, tick.high);
            Assert.AreEqual(101, tick.last);
            Assert.AreEqual(200, tick.last_volume);
            Assert.AreEqual(1, tick.lot_size);
            Assert.AreEqual(101, tick.low);
            Assert.AreEqual(0, tick.open);
            Assert.AreEqual(6605400, tick.turnover);
            Assert.AreEqual(65400, tick.turnover_volume);
            Assert.Pass("took: " + stopwatch.GetTimeString());
            //}
        }
    }
}
