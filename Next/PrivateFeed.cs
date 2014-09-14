namespace Next
{
    using System;

    using Next.Dtos;

    public class PrivateFeed : NextFeed
    {
        private const string cmdHeartbeat = @"{""type"":""heartbeat""";

        internal PrivateFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo)
            : base(client, feedInfo)
        {
        }

        protected override void OnReceivedSomething(string message)
        {
            base.OnReceivedSomething(message);
            if (message.StartsWith(cmdHeartbeat))
            {
                LastHeartBeatTime = DateTime.UtcNow;
                return;
            }
            OnReceivedUnknownMessage(message);
        }
    }
}