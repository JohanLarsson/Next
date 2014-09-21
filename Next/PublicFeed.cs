using System;
using System.Threading.Tasks;
using Next.Dtos;
using Next.FeedCommands;

namespace Next
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    public class PublicFeed : NextFeed
    {
        private const string Heartbeat = @"{""cmd"":""heartbeat""";
        private const string Err = @"{""cmd"":""err""";
        private const string Price = @"{""cmd"":""price""";
        private const string Depth = @"{""cmd"":""depth""";
        private const string Trade = @"{""cmd"":""trade""";
        private const string TradingStatus = @"{""cmd"":""trading_status""";
        private const string Index = @"{""cmd"":""index""";
        private const string News = @"{""cmd"":""news""";
        private readonly JsonSerializerSettings _serializerSettings;
        internal PublicFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo)
            : base(client, feedInfo)
        {
            _serializerSettings = new JsonSerializerSettings
                                      {
                                          MissingMemberHandling = MissingMemberHandling.Error,
                                          ContractResolver = new NextContractResolver()
                                      };
        }

        public event EventHandler<PriceTick> RecievedPrice;

        public event EventHandler<DepthTick> RecievedDepth;

        public event EventHandler<TradeTick> RecievedTrade;

        public event EventHandler<TradingStatusTick> RecievedTradingStatus;

        public event EventHandler<IndexTick> RecievedIndex;

        public event EventHandler<NewsTick> RecievedNews;

        public async Task Subscribe(InstrumentDescriptor instrument)
        {
            FeedCommand<SubscribeInstrumentArgsBase>[] feedCommands = FeedCommand.SubscribeAll(instrument);
            foreach (var cmd in feedCommands)
            {
                await Write(cmd);
            }
        }

        public async Task UnSubscribe(InstrumentDescriptor instrument)
        {
            FeedCommand<SubscribeInstrumentArgsBase>[] feedCommands = FeedCommand.UnSubscribeAll(instrument);
            foreach (var cmd in feedCommands)
            {
                await Write(cmd);
            }
        }

        protected override void OnReceivedSomething(string message)
        {
            base.OnReceivedSomething(message);
            try
            {
                //var jObject = JObject.Parse(message);
                //var jToken = o["cmd"];
                //var token = o["args"];
                //var price = token.ToObject<Price>();
                if (message.StartsWith(Depth))
                {
                    var tick = Deserialize<DepthTick>(message);
                    OnRecievedDepth(tick);
                    return;
                }

                if (message.StartsWith(Price))
                {
                    var tick = Deserialize<PriceTick>(message);
                    OnRecievedPrice(tick);
                    return;
                }

                if (message.StartsWith(Trade))
                {
                    var tick = Deserialize<TradeTick>(message);
                    OnRecievedTrade(tick);
                    return;
                }

                if (message.StartsWith(TradingStatus))
                {
                    var tick = Deserialize<TradingStatusTick>(message);
                    OnRecievedTradingStatus(tick);
                    return;
                }

                if (message.StartsWith(Index))
                {
                    var tick = Deserialize<IndexTick>(message);
                    OnRecievedIndex(tick);
                    return;
                }

                if (message.StartsWith(News))
                {
                    var tick = Deserialize<NewsTick>(message);
                    OnRecievedNews(tick);
                    return;
                }

                if (message.StartsWith(Heartbeat))
                {
                    LastHeartBeatTime = DateTime.UtcNow;
                    return;
                }
                if (message.StartsWith(Err))
                {
                    OnReceivedError(message);
                    return;
                }
            }
            catch (Exception e)
            {
                OnException(new ExceptionEventArgs(message, e));
            }

            OnReceivedUnknownMessage(message);
        }

        protected virtual void OnRecievedPrice(PriceTick e)
        {
            var handler = RecievedPrice;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRecievedDepth(DepthTick e)
        {
            var handler = RecievedDepth;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRecievedTrade(TradeTick e)
        {
            var handler = RecievedTrade;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRecievedTradingStatus(TradingStatusTick e)
        {
            var handler = RecievedTradingStatus;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRecievedIndex(IndexTick e)
        {
            var handler = RecievedIndex;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRecievedNews(NewsTick e)
        {
            var handler = RecievedNews;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private T Deserialize<T>(string message)
        {
            const string value = @"""args"":";
            int start = message.IndexOf(value, System.StringComparison.OrdinalIgnoreCase) + value.Length;
            int length = message.Length - start - 1;
            string json = message.Substring(start, length);
            var o = JsonConvert.DeserializeObject<T>(json, _serializerSettings);
            return o;
        }
    }
}