namespace Next
{
    public class LoginResult
    {
        public string Country { get; set; }
        public int expires_in { get; set; }
        public string session_key { get; set; }
        public FeedInfo private_feed { get; set; }
        public string environment { get; set; }
        public FeedInfo public_feed { get; set; }
    }
}