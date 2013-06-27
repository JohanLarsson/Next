namespace Next.Dtos
{
    public class RelatedMarket
    {
        public int MarketID { get; set; }
        public string Identifier { get; set; }

        public override string ToString()
        {
            return string.Format("MarketID: {0}, Identifier: {1}", MarketID, Identifier);
        }
    }
}