using System;

namespace Next.Dtos
{
    public class TradingStatusTick :InstrumentTick
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string SourceStatus { get; set; }
    }
}