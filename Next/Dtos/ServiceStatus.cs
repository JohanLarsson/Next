namespace Next.Dtos
{
    public class ServiceStatus
    {
        public string Message { get; set; }
        public bool ValidVersion { get; set; }
        public bool SystemRunning { get; set; }
        public bool SkipPhrase { get; set; }
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return string.Format("message: {0}, valid_version: {1}, system_running: {2}, skip_phrase: {3}, timestamp: {4}", Message, ValidVersion, SystemRunning, SkipPhrase, Timestamp);
        }
    }
}