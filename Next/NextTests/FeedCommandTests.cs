using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;
using Next.Dtos;
using Next.FeedCommands;

namespace NextTests
{
    public class FeedCommandTests
    {
        [Test]
        public void LoginTest()
        {
            string cmd = FeedCommand.LoginCommandParameter;
            const string service = "service";
            const string sessionKey = "key";
            FeedCommand<LoginArgs> loginCommand = FeedCommand.Login(service, sessionKey);
            Assert.AreEqual(cmd,loginCommand.cmd);
            Assert.AreEqual(service, loginCommand.args.service);
            Assert.AreEqual(sessionKey, loginCommand.args.session_key);
            string expected = string.Format(@"{{""cmd"":""{0}"",""args"":{{""session_key"":""{1}"",""service"":""{2}""}}}}", cmd, sessionKey, service);
            Assert.AreEqual(expected,loginCommand.ToJson());
            Console.WriteLine(loginCommand.ToJson());
        }

        [Test]
        public void SubscribeAllTest()
        {
            string cmd = FeedCommand.SubscribeCommandParameter;
            int marketId = 1;
            string identifier = "id";

            FeedCommand<SubscribeInstrumentArgsBase>[] feedCommands = FeedCommand.SubscribeAll(new InstrumentDescriptor(marketId, identifier));

            var expecteds = new[]
                {
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "price", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "depth", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "trade", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "index", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "trading_status", identifier, marketId),
                };

            string[] serializeds = feedCommands.Select(x => x.ToJson()).ToArray();
            for (int index = 0; index < expecteds.Length; index++)
            {
                var expected = expecteds[index];
                string actual = serializeds[index];
                Assert.AreEqual(expected,actual);
            }
            Assert.IsTrue(expecteds.SequenceEqual(serializeds));
            foreach (var feedCommand in feedCommands)
            {
                Console.WriteLine(feedCommand.ToJson());
            }
        }

        [Test, Explicit]
        public void UnSubscribeAllTest()
        {
            string cmd = FeedCommand.UnSubscribeCommandParameter;
            int marketId = 1;
            string identifier = "id";

            FeedCommand<SubscribeInstrumentArgsBase>[] feedCommands = FeedCommand.UnSubscribeAll(new InstrumentDescriptor(marketId, identifier));

            var expected = new[]
                {
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "price", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "depth", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "trade", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "index", identifier, marketId),
                    string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""i"":""{2}"",""m"":{3}}}}}", cmd, "trading_status", identifier, marketId),
                };

            Assert.IsTrue(expected.SequenceEqual(feedCommands.Select(x => x.ToJson())));
            foreach (var feedCommand in feedCommands)
            {
                Console.WriteLine(feedCommand.ToJson());
            }
        }

        [Test]
        public void NewsTest()
        {
            string cmd = "subscribe";
            int sourceid = 1;

            FeedCommand<SubscribeNewsArgs> subscribeNews = FeedCommand.SubscribeNews(new NewsSource() {Sourceid = sourceid});
            string expected = string.Format(@"{{""cmd"":""{0}"",""args"":{{""t"":""{1}"",""s"":{2},""delay"":{3}}}}}", cmd, SubscribeType.News, sourceid,"false");
            Assert.AreEqual(expected,subscribeNews.ToJson());
            Console.WriteLine(subscribeNews.ToJson());
        }
    }
}
