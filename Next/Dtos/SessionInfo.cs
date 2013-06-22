namespace Next.Dtos
{
    public class SessionInfo
    {
        public string Country { get; set; }
        public int ExpiresIn { get; set; }
        public string SessionKey { get; set; }
        public FeedInfo PrivateFeed { get; set; }
        public string Environment { get; set; }
        public FeedInfo PublicFeed { get; set; }
    }
}