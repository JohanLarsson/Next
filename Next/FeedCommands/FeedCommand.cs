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
                cmd = "login",
                args = new LoginArgs
                {
                    service = service,
                    session_key = sessionKey
                }
            };
        }

        private const string _subscribe = "subscribe";
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
                    cmd = _subscribe,
                    args = new SubscribeTradingStatusArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeIndex(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = _subscribe,
                    args = new SubscribeIndexArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeTrade(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = _subscribe,
                    args = new SubscribeTradeArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribeDepth(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = _subscribe,
                    args = new SubscribeDepthArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeInstrumentArgsBase> SubscribePrice(InstrumentDescriptor instrument)
        {
            return new FeedCommand<SubscribeInstrumentArgsBase>
                {
                    cmd = _subscribe,
                    args = new SubscribePriceArgs(instrument)
                };
        }

        public static FeedCommand<SubscribeNewsArgs> SubscribeNews(NewsSource newsSource, bool delay = false)
        {
            return new FeedCommand<SubscribeNewsArgs>
            {
                cmd = _subscribe,
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
            return serializer.Serialize(this);
        }

        public override string ToString()
        {
            return string.Format("cmd: {0}, args: {{{1}}}", cmd, args);
        }
    }
}
