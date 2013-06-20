namespace Next
{
    public class GenericMessage
    {
        public string message { get; set; }
        public bool valid_version { get; set; }
        public bool system_running { get; set; }
        public bool skip_phrase { get; set; }
        public long timestamp { get; set; }
    }
}