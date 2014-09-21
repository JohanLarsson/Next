namespace NextTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Next;
    using Next.Dtos;

    using NextTests.Mocks;

    using NUnit.Framework;

    public class PublicFeedTests
    {
        private PublicFeedStub _feed;

        private List<ExceptionEventArgs> _exceptions;

        [SetUp]
        public void SetUp()
        {
            _feed = new PublicFeedStub();
            _exceptions = new List<ExceptionEventArgs>();
            _feed.Exception += (_, e) => _exceptions.Add(e);
        }

        [TearDown]
        public void TearDown()
        {
            if (_exceptions.Any())
            {
                foreach (var exception in _exceptions)
                {
                    Console.WriteLine(exception.Exception.Message);
                }
            }
        }

        [Test]
        public void RecievePriceMessage()
        {
            var args = new List<PriceTick>();
            _feed.RecievedPrice += (_, e) => args.Add(e);
            _feed.ReceiveMessage(Properties.Resources.PublicFeedPriceMessage);
            Assert.AreEqual(1, args.Count);
        }

        [Test]
        public void RecieveDepthMessage()
        {
            var args = new List<DepthTick>();
            _feed.RecievedDepth += (_, e) => args.Add(e);
            _feed.ReceiveMessage(Properties.Resources.PublicFeedDepthMessage);
            Assert.AreEqual(1, args.Count);
        }

        [Test]
        public void RecieveTradeMessage()
        {
            var args = new List<TradeTick>();
            _feed.RecievedTrade += (_, e) => args.Add(e);
            _feed.ReceiveMessage(Properties.Resources.PublicFeedTradeMessage);
            Assert.AreEqual(1, args.Count);
        }

        [Test]
        public void RecieveTradingStatusMessage()
        {
            var args = new List<TradingStatusTick>();
            _feed.RecievedTradingStatus += (_, e) => args.Add(e);
            _feed.ReceiveMessage(Properties.Resources.PublicFeedTradingStatusMessage);
            Assert.AreEqual(1, args.Count);
        }

        [Test]
        public void RecieveIndexMessage()
        {
            var args = new List<IndexTick>();
            _feed.RecievedIndex += (_, e) => args.Add(e);
            _feed.ReceiveMessage(Properties.Resources.PublicFeedIndexMessage);
            Assert.AreEqual(1, args.Count);
        }

        [Test]
        public void RecieveNewsMessage()
        {
            var args = new List<NewsTick>();
            _feed.RecievedNews += (_, e) => args.Add(e);
            string json = Properties.Resources.PublicFeedNewsMessage;
            Console.WriteLine(json);
            _feed.ReceiveMessage(json);
            Assert.AreEqual(1, args.Count);
            var tick = args.Single();
            Assert.AreEqual(40705006, tick.Itemid);
            Assert.AreEqual("da", tick.Lang);
            Assert.AreEqual(new DateTime(2010, 12, 08, 22, 15, 00), tick.Datetime);
            Assert.AreEqual(6, tick.Sourceid);
            Assert.AreEqual("Amerikanske aktieindeks kl. 22:15", tick.Headline);
            var instrument = tick.Instruments.Single();
            Assert.AreEqual(11, instrument.MarketId);
            Assert.AreEqual("101", instrument.Identifier);
        }

        [Test]
        public void RecieveErrorMessage()
        {
            var args = new List<string>();
            _feed.ReceivedError += (_, e) => args.Add(e);
            var json = Properties.Resources.PublicFeedErrorMessage;
            Console.WriteLine(json);
            _feed.ReceiveMessage(json);
            Assert.AreEqual(1, args.Count);
        }

        [Test]
        public void RecieveExceptionMessage()
        {
            var args = new List<ExceptionEventArgs>();
            _feed.Exception += (_, e) => args.Add(e);
            _feed.ReceiveMessage(Properties.Resources.PublicFeedMalformedMessage);
            Assert.AreEqual(1, args.Count);
        }

        [Test, Explicit]
        public void TimeReceiving1MillionMessages()
        {
            const string format = @"{{""cmd"":""depth"",""args"":{{""i"":""{0}"",""m"":30,""t"":""depth"",""tick_timestamp"":""2010-01-22 16:22:06"",""bid1"":0.00,""bid_volume1"":0,""ask1"":120.00,""ask_volume1"":200,""bid2"":0.00,""bid_volume2"":0,""ask2"":0.00,""ask_volume2"":0,""bid3"":0.00,""bid_volume3"":0,""ask3"":0.00,""ask_volume3"":0,""bid4"":0.00,""bid_volume4"":0,""ask4"":0.00,""ask_volume4"":0,""bid5"":0.00,""bid_volume5"":0,""ask5"":0.00,""ask_volume5"":0}}}}";
            const int n = 1000000;
            var messages = new string[n];
            for (int i = 0; i < n; i++)
            {
                messages[i] = string.Format(format, i);
            }
            var feed = new PublicFeedStub();
            var args = new List<DepthTick>(n);
            Stopwatch stopwatch = Stopwatch.StartNew();
            feed.RecievedDepth += (_, e) => args.Add(e);

            for (int i = 0; i < n; i++)
            {
                feed.ReceiveMessage(messages[i]);
            }
            Console.WriteLine("This took {0} ms {1}", stopwatch.ElapsedMilliseconds, DateTime.Now.ToShortDateString());
            // This took 24152 ms 2014-09-14
            Assert.AreEqual(n, args.Count);
        }
    }
}
