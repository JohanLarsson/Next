using System;
using System.Threading.Tasks;
using Next.Dtos;
using Next.FeedCommands;

namespace Next
{
    public class PublicFeed : NextFeed
    {
        public PublicFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo) : base(client, feedInfo)
        {
        }

        public async Task Subscribe(InstrumentDescriptor instrument)
        {
            FeedCommand<SubscribeArgsBase>[] feedCommands = FeedCommand.Subscribe(instrument);
            foreach (var subscribeCmd in feedCommands)
            {
                await Write(subscribeCmd);
            }
        }
    }
}