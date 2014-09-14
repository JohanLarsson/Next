namespace NextTests.Mocks
{
    using System;

    using Next;
    using Next.Dtos;

    public class PublicFeedStub : PublicFeed
    {
        internal PublicFeedStub()
            : base(null, null)
        {
        }
        internal PublicFeedStub(NextClient client, Func<NextClient, FeedInfo> feedInfo)
            : base(client, feedInfo)
        {
        }

        public void ReceiveMessage(string message)
        {
            this.OnReceivedSomething(message);
        }
    }
}
