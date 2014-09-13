using System;
using RestSharp.Deserializers;

namespace Next.Dtos
{
    public abstract class InstrumentTick : ITick
    {
        protected InstrumentTick()
        {
            Instrument= new InstrumentDescriptor(null, null);
        }
        public InstrumentDescriptor Instrument { get; set; }
        public string I { get { return Instrument.Identifier; } set { Instrument.Identifier = value; } }
        public string M { get { return Instrument.MarketId; } set { Instrument.MarketId = value; } }
        public string T { get; set; }
    }
}