namespace Next.Dtos
{
    public class InstrumentList
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Country: {1}, Id: {2}", Name, Country, Id);
        }
    }
}