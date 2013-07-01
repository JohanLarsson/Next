using System.Text.RegularExpressions;
using Next.Dtos;
using RestSharp.Deserializers;

namespace Next.FeedCommands
{
    public class PublicFeedMessage
    {
        private static readonly Regex priceTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.Price),
                                                        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsPriceTick(string json)
        {
            return priceTickRegex.IsMatch(json);
        }

        private static readonly Regex depthTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.Depth),
                                                        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsDepthTick(string json)
        {
            return depthTickRegex.IsMatch(json);
        }

        private static readonly Regex tradeTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.Trade),
                                                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsTradeTick(string json)
        {
            return tradeTickRegex.IsMatch(json);
        }

        private static readonly Regex tradingStatusTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.TradingStatus),
                                        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsTradingStatusTick(string json)
        {
            return tradingStatusTickRegex.IsMatch(json);
        }

        private static readonly Regex indexTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.Index),
                                        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsIndexTick(string json)
        {
            return indexTickRegex.IsMatch(json);
        }


        private static readonly Regex newsTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.News),
                                        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsNewsTick(string json)
        {
            return newsTickRegex.IsMatch(json);
        }

        public static T CreateTick<T>(string json) where T : ITick
        {
            JsonDeserializer deserializer = new JsonDeserializer();
            PublicFeedMessage<T> message = deserializer.Deserialize<PublicFeedMessage<T>>(new FakeResponse(json));
            T tick = message.Args;
            return tick;
        }


    }
}