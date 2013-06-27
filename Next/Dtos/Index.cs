namespace Next.Dtos
{
    public class Index
    {
        public string Type { get; set; }
        public string Longname { get; set; }
        public string Source { get; set; }
        public string Country { get; set; }
        public string Imageurl { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, Longname: {1}, Source: {2}, Country: {3}, Imageurl: {4}, Id: {5}", Type, Longname, Source, Country, Imageurl, Id);
        }
    }
}