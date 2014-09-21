namespace Next
{
    public class PublicFeedMessageHandler
    {
        public PublicFeedMessageHandler(PublicFeed feed)
        {
            Feed = feed;
        }

        public void ReceivedMesage(string message)
        {
            
        }

        public PublicFeed Feed { get; set; }
    }
}
