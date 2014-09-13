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

        protected override void OnReceivedSomething(string s)
        {
            base.OnReceivedSomething(s);
            if (s.StartsWith(cmdHeartbeat))
            {
                this.LastHeartBeatTime = DateTime.UtcNow;
                return;
            }
            OnReceivedUnknownMessage(s);
        }
    }
}