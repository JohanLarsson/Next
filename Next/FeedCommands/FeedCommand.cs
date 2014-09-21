using System.Collections.Generic;
using Next.Dtos;

namespace Next.FeedCommands
{
    /// <summary>
    /// https://api.test.nordnet.se/projects/api/wiki/Feed_API_documentation#Subgroup-Subscribe-request
    /// </summary>
    public class FeedCommand
    {
        public static FeedCommand<LoginArgs> Login(string service, string sessionKey)
        {
            return new FeedCommand<LoginArgs>()
            {
                cmd = LoginCommandParameter,
                args = new LoginArgs
                {
                    service = service,
                    session_key = sessionKey
                }
            };
        }

        public const string SubscribeCommandParameter = "subscribe";
        public const string UnSubscribeCommandParameter = "unsubscribe";
        public const string LoginCommandParameter = "login";

        /// <summary>
        /// Returns an array of commands to subscribe everything for instrument
        /// https://api.test.nordnet.se/projects/api/wiki/Feed_API_documentation#The-public-trade-and-price-feed
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public static FeedCommand<SubscribeInstrumentArgsBase>[] SubscribeAll(InstrumentDescriptor instrument)
        {
            return new[]
                {
                    SubscribePrice(instrument),
                    SubscribeDepth(instrument),
                    SubscribeTrade(instrument),
                    SubscribeIndex(instrument),
                    SubscribeTradingStatus(instrument),
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeTradingStatus(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = SubscribeCommandParameter,
                    args = new SubscribeTradingStatusArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeIndex(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = SubscribeCommandParameter,
                    args = new SubscribeIndexArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeTrade(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = SubscribeCommandParameter,
                    args = new SubscribeTradeArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeDepth(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = SubscribeCommandParameter,
                    args = new SubscribeDepthArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribePrice(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = SubscribeCommandParameter,
                    args = new SubscribePriceArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeNewsArgs> SubscribeNews(NewsSource newsSource, bool delay = false)
        {
            return new FeedCommand<SubscribeNewsArgs>
            {
                cmd = SubscribeCommandParameter,
                args = new SubscribeNewsArgs(newsSource, delay)
            };
        }

        /// <summary>
        /// Returns an array of commands to subscribe everything for instrument
        /// https://api.test.nordnet.se/projects/api/wiki/Feed_API_documentation#The-public-trade-and-price-feed
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public static FeedCommand<SubscribeInstrumentArgsBase>[] UnSubscribeAll(InstrumentDescriptor instrument)
        {
            return new[]
                {
                   UnSubscribePrice(instrument),
                   UnSubscribeDepth(instrument),
                   UnSubscribeTrade(instrument),
                   UnSubscribeIndex(instrument),
                   UnSubscribeTradingStatus(instrument),
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> UnSubscribeTradingStatus(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
            {
                cmd = UnSubscribeCommandParameter,
                args = new SubscribeTradingStatusArgs(instrument)
            };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> UnSubscribeIndex(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
            {
                cmd = UnSubscribeCommandParameter,
                args = new SubscribeIndexArgs(instrument)
            };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> UnSubscribeTrade(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
            {
                cmd = UnSubscribeCommandParameter,
                args = new SubscribeTradeArgs(instrument)
            };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> UnSubscribeDepth(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
            {
                cmd = UnSubscribeCommandParameter,
                args = new SubscribeDepthArgs(instrument)
            };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> UnSubscribePrice(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
            {
                cmd = UnSubscribeCommandParameter,
                args = new SubscribePriceArgs(instrument)
            };
        }

        public static FeedCommand<SubscribeNewsArgs> UnSubscribeNews(NewsSource newsSource, bool delay = false)
        {
            return new FeedCommand<SubscribeNewsArgs>
            {
                cmd = UnSubscribeCommandParameter,
                args = new SubscribeNewsArgs(newsSource, delay)
            };
        }
    }

    public class FeedCommand<T> : FeedCommand
    {
        public string cmd { get; set; }
        public T args { get; set; }

        public string ToJson()
        {
            var serializer = new RestSharp.Serializers.JsonSerializer();
            string json = serializer.Serialize(this);
            return json;
        }

        public override string ToString()
        {
            return string.Format("cmd: {0}, args: {{{1}}}", cmd, args);
        }
    }
}
