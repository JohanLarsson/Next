using System;

namespace Next.Dtos
{
    public class TradeTick :InstrumentTick
    {
        public DateTime TradeTimestamp { get; set; }
        public double Price { get; set; }
        public int Volume { get; set; }
        public double Baseprice { get; set; }
        public string BrokerBuying { get; set; }
        public string BrokerSelling { get; set; }
    }
}