using System;

namespace Next.Dtos
{
    public class IndexTick : InstrumentTick
    {
        public DateTime TickTimestamp { get; set; }
        public decimal Last { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}