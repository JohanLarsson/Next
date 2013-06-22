namespace Next.Dtos
{
    public class InstrumentMatch : Instrument
    {
        public string Longname { get; set; }
        public string marketID { get { return base.MainMarketId; } set { base.MainMarketId = value; } }
        public string Country { get; set; }
        public string Shortname { get; set; }
        public string Marketname { get; set; }
        public string IsinCode { get; set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, Longname: {1}, marketID: {2}, Country: {3}, Shortname: {4}, Marketname: {5}, IsinCode: {6}, Identifier: {7}, Currency: {8}", Type, Longname, marketID, Country, Shortname, Marketname, IsinCode, Identifier, Currency);
        }
    }
}