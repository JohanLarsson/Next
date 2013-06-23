﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using Next;
using Next.Dtos;
using NextTests.Prototypes;
using RestSharp;
using NextTests.Helpers;

namespace NextTests
{
    [Explicit]
    public class NextClientTests
    {
        [Test]
        public async Task ServiceStatusTest()
        {
            var client = new NextClient(ApiVersion.Test);
            ServiceStatus serviceStatus = await client.ServiceStatus();
            Assert.IsTrue(serviceStatus.SystemRunning);
            Assert.IsTrue(serviceStatus.ValidVersion);
        }

        [Test]
        public async Task LoginTest()
        {
            var client = new NextClient(ApiVersion.Test);

            bool success = await client.Login(Credentials.Username, Credentials.Password);

            Assert.IsTrue(success);
            Assert.IsNotNull(client.Session.SessionKey);
            Assert.IsTrue(client.Session.ExpiresIn > 0);
            Assert.AreEqual("test", client.Session.Environment);
            Assert.IsNotNull(client.Session.SessionKey);
            Assert.IsNotNull(client.Session.Country);
        }

        [Test]
        public async Task LoginFailTest()
        {
            var nextClient = new NextClient(ApiVersion.Test);
            Assert.IsFalse(await nextClient.Login("Incorrect", "Credentials"));
        }

        [Test]
        public async Task LogoutTest()
        {
            var client = LoggedInClient;
            Assert.IsTrue(await client.Logout());
            Assert.IsNull(client.Session);
        }

        [Test]
        public async Task LogoutWhenNotLoggedInTest()
        {
            var client = new NextClient(ApiVersion.Test);
            Assert.IsTrue(await client.Logout());
        }

        [Test]
        public async Task TouchTest()
        {
            Assert.IsTrue(await LoggedInClient.Touch());
        }

        [Test]
        public async Task RealtimeAccessTest()
        {
            List<RealtimeAccesMarket> markets = await LoggedInClient.RealtimeAccess();
            Assert.IsTrue(markets.Any());
            Assert.IsTrue(markets.All(m => m.MarketID != null));
        }

        [Test]
        public async Task NewsSourcesTest()
        {
            List<NewsSource> newsSources = await LoggedInClient.NewsSources();
            Assert.IsTrue(newsSources.Any());
            Assert.IsTrue(newsSources.All(m => m.Code != null && m.Name != null && m.Sourceid != 0));
        }

        [Test]
        public void SearchNewsTest()
        {
            string news = LoggedInClient.SearchNews();
            Console.WriteLine(news);
            Assert.Inconclusive("Not sure this is working with test think code is ok");
        }

        [Test]
        public void NewsItemTest()
        {
            string s = LoggedInClient.NewsItem("4711");
            Console.WriteLine(s);
            Assert.Inconclusive("Not sure this is working with test think code is ok");
        }

        [Test]
        public async Task AccountsTest()
        {
            var accounts = await LoggedInClient.Accounts();
            Assert.IsTrue(accounts.Any());
            Assert.IsTrue(accounts.All(a => a.Id != null));
        }

        [Test]
        public async Task AccountSummaryTest()
        {
            AccountSummary summary = await LoggedInClient.AccountSummary(Accounts.First());
            Assert.IsFalse(string.IsNullOrEmpty(summary.AccountCurrency));
        }

        [Test]
        public async Task AccountLedgersItemTest()
        {
            List<Ledger> ledgers = await LoggedInClient.AccountLedgers(Accounts.First());
            Assert.Inconclusive("how to test this?");
        }

        [Test]
        public async Task AccountPositionsTest()
        {
            List<Position> positions = await LoggedInClient.AccountPositions(Accounts.First());
            Assert.Inconclusive("how to test this?");
        }

        [Test]
        public async Task AccountOrdersTest()
        {
            List<Order> orders = await LoggedInClient.AccountOrders(Accounts.First());
            Assert.Fail("Probably need to place an order before fetching this Order class is empty shell lacking json");
            Assert.Inconclusive("how to test this?");
        }

        [Test]
        public async Task AccountTradesTest()
        {
            List<Trade> trades = await LoggedInClient.AccountTrades(Accounts.First());
            Assert.Inconclusive("how to test this?");
        }

        [Test]
        public async Task InstrumentSearchQueryTest()
        {
            List<InstrumentMatch> matches = await LoggedInClient.InstrumentSearch("ERI");
            Assert.AreEqual(4, matches.Count);
            Assert.IsTrue(matches.All(m =>
                                      m.Country != null &&
                                      m.Currency != null &&
                                      m.Identifier != null &&
                                      m.IsinCode != null &&
                                      m.Longname != null &&
                                      m.MainMarketId != null &&
                                      m.Marketname != null &&
                                      m.Shortname != null &&
                                      m.Type != null &&
                                      m.marketID != null
                              ));
        }

        [Test]
        public async Task InstrumentSearchIdentifierTest()
        {
            InstrumentMatch match = await LoggedInClient.InstrumentSearch(Ericcson.Identifier, int.Parse(Ericcson.marketID));
            Assert.AreEqual(Ericcson, match);
        }

        [Test]
        public async Task InstrumentSearchMultiIdentifierTest()
        {
            List<InstrumentMatch> matches = await LoggedInClient.InstrumentSearch(new InstrumentDescriptor[]
                {
                    new InstrumentDescriptor(11, "101"),
                    new InstrumentDescriptor(11, "817")
                });
            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual(Ericcson, matches.First());
        }

        [Test]
        public async Task ChartDataTest()
        {
            List<Tick> ticks = await LoggedInClient.ChartData(Ericcson.Identifier, int.Parse(Ericcson.marketID));
            Assert.Inconclusive("Guess market must be open to test this");
        }

        [Test]
        public async Task InstrumentListsTest()
        {
            List<InstrumentList> lists = await LoggedInClient.Lists();
            Assert.AreEqual(96, lists.Count);
        }

        [Test]
        public async Task ListItemsTest()
        {
            List<InstrumentItem> items = await LoggedInClient.ListItems(8.ToString());
            Assert.AreEqual(78, items.Count);
            Assert.IsTrue(items.All(x =>
                                    x.Identifier != null &&
                                    x.MarketID != null &&
                                    x.Shortname != null));
        }

        [Test]
        public async Task MarketsTest()
        {
            List<Market> markets = await LoggedInClient.Markets();
            Assert.AreEqual(36, markets.Count);
            Assert.IsTrue(markets.All(x =>
                                      x.Country != null &&
                                      x.MarketID != null &&
                                      x.Name != null
                                          ));
            Assert.IsTrue(markets.Any(x => x.Ordertypes.All(o =>
                                 o.Text != null &&
                                 o.Type != null
                    )));
        }

        [Test]
        public async Task TradingDaysTest()
        {
            List<TradingDay> tradingDays = await LoggedInClient.TradingDays(8);
            Assert.AreEqual(65, tradingDays.Count);
            Assert.IsTrue(tradingDays.All(x =>
                                          x.Date != default(DateTime) &&
                                          x.DisplayDate != default(DateTime)));

        }

        [Test]
        public async Task IndicesTest()
        {
            List<Index> indices = await LoggedInClient.Indices();
            Assert.AreEqual(59, indices.Count);
            Assert.IsTrue(indices.All(x =>
                                          x.Id != null &&
                                          x.Longname != null &&
                                          x.Source != null &&
                                          x.Type != null
                                          ));
            Assert.IsTrue(indices.Any(x => x.Imageurl != null));
            Assert.IsTrue(indices.Any(x => x.Country != null));
        }

        [Test]
        public async Task TickSizesTest()
        {
            List<TickSize> tickSizes = await LoggedInClient.TickSizes(Ericcson.Identifier);
            Assert.AreEqual(1, tickSizes.Count);
        }

        private NextClient LoggedInClient
        {
            get
            {
                var nextClient = new NextClient(ApiVersion.Test);
                Assert.IsTrue(nextClient.Login(Credentials.Username, Credentials.Password).Result); // this hangs
                return nextClient;
            }
        }

        private List<Account> Accounts
        {
            get
            {
                List<Account> accounts = LoggedInClient.Accounts().Result;
                return accounts;
            }
        }

        private InstrumentMatch Ericcson
        {
            get { return Properties.Settings.Default.EricssonInstrumentMatch; }
        }

        public Credentials Credentials { get { return Credentials.Load(Properties.Resources.CredentialsPath); } }

    }
}
