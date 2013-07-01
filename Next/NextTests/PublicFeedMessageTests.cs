using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;
using Next.Dtos;
using Next.FeedCommands;
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
            PriceTick tick = PublicFeedMessage.CreateTick<PriceTick>(priceTick);
            Console.WriteLine("PublicFeedMessage.DeserializePriceTick(priceTick) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());
            Assert.AreEqual(new InstrumentDescriptor(30, "1843"), tick.Instrument);
            Assert.AreEqual(new DateTime(2010, 01, 22, 16, 17, 23), tick.TradeTimestamp);
            Assert.AreEqual(new DateTime(2010, 01, 22, 16, 18, 42), tick.TickTimestamp);

            Assert.AreEqual(0, tick.Bid);
            Assert.AreEqual(0, tick.BidVolume);
            Assert.AreEqual(101, tick.Ask);
            Assert.AreEqual(200, tick.AskVolume);
            Assert.AreEqual(101, tick.Close);
            Assert.AreEqual(101, tick.Close);
            Assert.AreEqual(101, tick.High);
            Assert.AreEqual(101, tick.Last);
            Assert.AreEqual(200, tick.LastVolume);
            Assert.AreEqual(1, tick.LotSize);
            Assert.AreEqual(101, tick.Low);
            Assert.AreEqual(0, tick.Open);
            Assert.AreEqual(6605400, tick.Turnover);
            Assert.AreEqual(65400, tick.TurnoverVolume);
            Assert.Pass("took: " + stopwatch.GetTimeString());
            //}
        }

        [Test]
        public void DepthTickTest()
        {
            string depthTickJson = Properties.Settings.Default.DepthTickJson;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch deserializeSw = Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            stopwatch.Restart();

            Assert.IsTrue(PublicFeedMessage.IsDepthTick(depthTickJson));
            Console.WriteLine("PublicFeedMessage.IsDepthTick(depthTickJson) took: " + stopwatch.GetTimeString());
            deserializeSw.Restart();
            DepthTick tick = PublicFeedMessage.CreateTick<DepthTick>(depthTickJson);
            Console.WriteLine("PublicFeedMessage.DeserializeDepthTick(depthTickJson) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());
            Assert.AreEqual(new InstrumentDescriptor(30, "2188"), tick.Instrument);
            Assert.AreEqual(new DateTime(2010, 01, 22, 16, 22, 06), tick.TickTimestamp);

            Assert.AreEqual(1, tick.Bid1);
            Assert.AreEqual(100, tick.BidVolume1);
            Assert.AreEqual(2, tick.Bid2);
            Assert.AreEqual(200, tick.BidVolume2);
            Assert.AreEqual(3, tick.Bid3);
            Assert.AreEqual(300, tick.BidVolume3);
            Assert.AreEqual(4, tick.Bid4);
            Assert.AreEqual(400, tick.BidVolume4);
            Assert.AreEqual(5, tick.Bid5);
            Assert.AreEqual(500, tick.BidVolume5);

            Assert.AreEqual(1.1, tick.Ask1);
            Assert.AreEqual(110, tick.AskVolume1);
            Assert.AreEqual(1.2, tick.Ask2);
            Assert.AreEqual(120, tick.AskVolume2);
            Assert.AreEqual(1.3, tick.Ask3);
            Assert.AreEqual(130, tick.AskVolume3);
            Assert.AreEqual(1.4, tick.Ask4);
            Assert.AreEqual(140, tick.AskVolume4);
            Assert.AreEqual(1.5, tick.Ask5);
            Assert.AreEqual(150, tick.AskVolume5);
            Assert.Pass("took: " + stopwatch.GetTimeString());

            
            //}
        }

        [Test]
        public void TradeTickTest()
        {
            string tickJson = Properties.Settings.Default.TradeTickJson;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch deserializeSw = Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            stopwatch.Restart();

            Assert.IsTrue(PublicFeedMessage.IsTradeTick(tickJson));
            Console.WriteLine("PublicFeedMessage.IsTradeTick(tickJson) took: " + stopwatch.GetTimeString());
            deserializeSw.Restart();
            TradeTick tick = PublicFeedMessage.CreateTick<TradeTick>(tickJson);
            Console.WriteLine("PublicFeedMessage.DeserializeDepthTick(depthTickJson) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());
            Assert.AreEqual(new InstrumentDescriptor(30, "1843"), tick.Instrument);
            Assert.AreEqual(new DateTime(2010, 01, 22, 16, 24, 04), tick.TradeTimestamp);

            Assert.AreEqual(101, tick.Price);
            Assert.AreEqual(200, tick.Volume);
            Assert.AreEqual(101, tick.Baseprice);
            Assert.AreEqual("ZCFT", tick.BrokerBuying);
            Assert.AreEqual("ZCFT", tick.BrokerSelling);

            Assert.Pass("took: " + stopwatch.GetTimeString());


            //}
        }

        [Test]
        public void TradingStatusTickTest()
        {
            string tickJson = Properties.Settings.Default.TradingStatusTickJson;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch deserializeSw = Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            stopwatch.Restart();

            Assert.IsTrue(PublicFeedMessage.IsTradingStatusTick(tickJson));
            Console.WriteLine("PublicFeedMessage.IsTradeTick(tickJson) took: " + stopwatch.GetTimeString());
            deserializeSw.Restart();
            TradingStatusTick tick = PublicFeedMessage.CreateTick<TradingStatusTick>(tickJson);
            Console.WriteLine("PublicFeedMessage.DeserializeDepthTick(depthTickJson) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());
            Assert.AreEqual(new InstrumentDescriptor(11, "4208"), tick.Instrument);
            Assert.AreEqual(new DateTime(2010, 01, 25, 09, 00, 00), tick.Timestamp);

            Assert.AreEqual("C", tick.Status);
            Assert.AreEqual("ContinuousTrading", tick.SourceStatus);

            Assert.Pass("took: " + stopwatch.GetTimeString());


            //}
        }

        [Test]
        public void IndexTickTest()
        {
            string tickJson = Properties.Settings.Default.IndexTickJson;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch deserializeSw = Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            stopwatch.Restart();

            Assert.IsTrue(PublicFeedMessage.IsIndexTick(tickJson));
            Console.WriteLine("PublicFeedMessage.IsTradeTick(tickJson) took: " + stopwatch.GetTimeString());
            deserializeSw.Restart();
            IndexTick tick = PublicFeedMessage.CreateTick<IndexTick>(tickJson);
            Console.WriteLine("PublicFeedMessage.DeserializeDepthTick(depthTickJson) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());
            Assert.AreEqual(new InstrumentDescriptor("SIX", "SIX-IDX-DJI"), tick.Instrument);
            Assert.AreEqual(new DateTime(2010, 09, 24, 07, 01, 58), tick.TickTimestamp);

            Assert.AreEqual(10662.4199m, tick.Last);
            Assert.AreEqual(10761.9404m, tick.High);
            Assert.AreEqual(10640.9199m, tick.Low);
            Assert.AreEqual(10739.31m, tick.Close);

            Assert.Pass("took: " + stopwatch.GetTimeString());


            //}
        }

        [Test]
        public void NewsTickTest()
        {
            string tickJson = Properties.Settings.Default.NewsTickJson;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Stopwatch deserializeSw = Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            stopwatch.Restart();

            Assert.IsTrue(PublicFeedMessage.IsNewsTick(tickJson));
            Console.WriteLine("PublicFeedMessage.IsTradeTick(tickJson) took: " + stopwatch.GetTimeString());
            deserializeSw.Restart();
            NewsTick tick = PublicFeedMessage.CreateTick<NewsTick>(tickJson);
            Console.WriteLine("PublicFeedMessage.DeserializeDepthTick(depthTickJson) took: " + deserializeSw.GetTimeString());
            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.GetTimeString());

            Assert.AreEqual(40705006, tick.Itemid);
            Assert.AreEqual("da", tick.Lang);
            Assert.AreEqual(new DateTime(2010, 12, 08, 22, 15, 00), tick.Datetime);

            Assert.AreEqual(6, tick.Sourceid);
            Assert.AreEqual("Amerikanske aktieindeks kl. 22:15", tick.Headline);
            Assert.IsTrue(new[] {new InstrumentDescriptor(11, "101")}.SequenceEqual(tick.Instruments));

            Assert.Pass("took: " + stopwatch.GetTimeString());


            //}
        }
    }
}
