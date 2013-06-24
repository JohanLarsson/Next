namespace Next.Dtos
{
    public class Trade
    {
        public string Volume { get; set; }
        public string Accno { get; set; }
        public IntrumentID IntrumentID { get; set; }
        public string Side { get; set; }
        public string TradeID { get; set; }
        public string Tradetime { get; set; }
        public Price Price { get; set; }
        public string Counterparty { get; set; }
        public string OrderID { get; set; }
    }
}