namespace Next.Dtos
{
    public class Position
    {
        public string AcqPriceAcc { get; set; }
        public string PawnPercent { get; set; }
        public string Qty { get; set; }
        public Instrument Instrument { get; set; }
        public string MarketValue { get; set; }
        public string MarketValueAcc { get; set; }
        public string AcqPrice { get; set; }

        public override string ToString()
        {
            return string.Format("AcqPriceAcc: {0}, PawnPercent: {1}, Qty: {2}, Instrument: {3}, MarketValue: {4}, MarketValueAcc: {5}, AcqPrice: {6}", AcqPriceAcc, PawnPercent, Qty, Instrument, MarketValue, MarketValueAcc, AcqPrice);
        }
    }
}