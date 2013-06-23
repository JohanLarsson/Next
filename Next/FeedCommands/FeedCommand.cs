namespace Next.FeedCommands
{
    public class FeedCommand
    {
        public static FeedCommand<LoginArgs> Login(string service, string sessionKey)
        {
            return new FeedCommand<LoginArgs>()
            {
                cmd = "login",
                args = new LoginArgs
                {
                    service = service,
                    session_key = sessionKey
                }
            };
        } 
    }

    public class FeedCommand<T> : FeedCommand
    {
        public string cmd { get; set; }
        public T args { get; set; }

        public string ToJson()
        {
            var serializer = new RestSharp.Serializers.JsonSerializer();
            return serializer.Serialize(this);
        }

        public override string ToString()
        {
            return string.Format("cmd: {0}, args: {{{1}}}", cmd, args);
        }
    }

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
