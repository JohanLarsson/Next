namespace Next.Dtos
{
    public class FeedInfo
    {
        public int Port { get; set; }
        public string Hostname { get; set; }
        public bool Encrypted { get; set; }

        public override string ToString()
        {
            return string.Format("Port: {0}, Hostname: {1}, Encrypted: {2}", Port, Hostname, Encrypted);
        }
    }
}