using System;
using System.Threading.Tasks;
using Next.Dtos;
using Next.FeedCommands;

namespace Next
{
    public class PublicFeed : NextFeed
    {
        private const string Heartbeat = @"{""cmd"":""heartbeat""";

        private const string Err = @"{""cmd"":""err""";

        internal PublicFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo) 
            : base(client, feedInfo)
        {
        }

        public async Task Subscribe(InstrumentDescriptor instrument)
        {
            FeedCommand<SubscribeInstrumentArgsBase>[] feedCommands = FeedCommand.SubscribeAll(instrument);
            foreach (var subscribeCmd in feedCommands)
            {
                await this.Write(subscribeCmd);
            }
        }

        protected override void OnReceivedSomething(string s)
        {
            base.OnReceivedSomething(s);
            if (s.StartsWith(Heartbeat))
            {
                this.LastHeartBeatTime = DateTime.UtcNow;
                return;
            }
            if (s.StartsWith(Err))
            {
                this.OnReceivedError(s);
                return;
            }
            OnReceivedUnknownMessage(s);
        }
    }
}