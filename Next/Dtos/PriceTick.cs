using System;

namespace Next.Dtos
{
    public class PriceTick :InstrumentTick
    {
        public DateTime TickTimestamp { get; set; }
        public DateTime TradeTimestamp { get; set; }
        public decimal Bid { get; set; }
        public int BidVolume { get; set; }
        public decimal Ask { get; set; }
        public int AskVolume { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Last { get; set; }
        public int LastVolume { get; set; }
        public int LotSize { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public int Turnover { get; set; }
        public int TurnoverVolume { get; set; }

    }
}