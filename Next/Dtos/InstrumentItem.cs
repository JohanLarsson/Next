namespace Next.Dtos
{
    public class InstrumentItem
    {
        public string Shortname { get; set; }
        public string MarketID { get; set; }
        public string Identifier { get; set; }

        public override string ToString()
        {
            return string.Format("Shortname: {0}, MarketID: {1}, Identifier: {2}", Shortname, MarketID, Identifier);
        }
    }
}