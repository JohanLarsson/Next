using System;
using System.Collections.Generic;

namespace Next.Dtos
{
    public class NewsTick :ITick
    {
        public long Itemid { get; set; }
        public string Lang { get; set; }
        public DateTime Datetime { get; set; }
        public int Sourceid { get; set; }
        public string Headline { get; set; }
        public List<InstrumentDescriptor> Instruments { get; set; }
    }
}