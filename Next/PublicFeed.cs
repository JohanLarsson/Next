using System;
using System.Threading.Tasks;
using Next.Dtos;
using Next.FeedCommands;

namespace Next
{
    public class PublicFeed : NextFeed
    {
        internal PublicFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo) 
            : base(client, feedInfo)
        {
        }

        public async Task Subscribe(InstrumentDescriptor instrument)
        {
            FeedCommand<SubscribeInstrumentArgsBase>[] feedCommands = FeedCommand.SubscribeAll(instrument);
            foreach (var subscribeCmd in feedCommands)
            {
                await Write(subscribeCmd);
            }
        }
    }
}