using System;
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
        }

        [Test]
        public void NewsItemTest()
        {
            string s = LoggedInClient.NewsItem("4711");
            Console.WriteLine(s);
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

        public Credentials Credentials { get { return Credentials.Load(Properties.Resources.CredentialsPath); } }

    }
}
