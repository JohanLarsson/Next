using System.Collections.Generic;

namespace Next.FeedCommands
{
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
        public static FeedCommand<SubscribeArgsBase>[] Subscribe(InstrumentDescriptor instrument)
        {
            return new[]
                {
                    new FeedCommand<SubscribeArgsBase>
                        {
                            cmd = _subscribe,
                            args = new SubscribePriceArgs(instrument)
                        },

                    new FeedCommand<SubscribeArgsBase>
                        {
                            cmd = _subscribe,
                            args = new SubscribeDepthArgs(instrument)
                        },
                    new FeedCommand<SubscribeArgsBase>
                        {
                            cmd = _subscribe,
                            args = new SubscribeTradeArgs(instrument)
                        },
                    new FeedCommand<SubscribeArgsBase>
                        {
                            cmd = _subscribe,
                            args = new SubscribeIndexArgs(instrument)
                        },
                    new FeedCommand<SubscribeArgsBase>
                        {
                            cmd = _subscribe,
                            args = new SubscribeTradingStatusArgs(instrument)
                        },
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
