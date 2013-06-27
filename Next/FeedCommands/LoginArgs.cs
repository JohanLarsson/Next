namespace Next.FeedCommands
{
    public class LoginArgs
    {
        public string session_key { get; set; }
        public string service { get; set; }

        public override string ToString()
        {
            return string.Format("session_key: {0}, service: {1}", session_key, service);
        }
    }
}