namespace Next.FeedCommands
{
    public class PublicFeedMessage<T>
    {
        public string Cmd { get; set; }

        public T Args { get; set; }
    }
}
