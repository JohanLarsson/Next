namespace NextView
{
    using System;

    public class FeedEvent
    {
        private FeedEvent(string readOrWrote, string message)
        {
            this.TimeStamp = DateTime.UtcNow;
            this.Action = readOrWrote;
            this.Message = message;
        }
        public static FeedEvent Wrote(string message)
        {
            return new FeedEvent("Wrote", message);
        }
        public static FeedEvent Read(string message)
        {
            return new FeedEvent("Read", message);
        }
        public DateTime TimeStamp { get; private set; }

        public string Action { get; private set; }

        public string Message { get; set; }
    }
}