namespace Next
{
    public class InstrumentDescriptor
    {
        public InstrumentDescriptor( int marketId,string identifier)
        {
            Identifier = identifier;
            MarketId = marketId;
        }

        public string Identifier { get; set; }
        public int MarketId {get; set; }

        public override string ToString()
        {
            return string.Format("MarketId: {1}, Identifier: {0}", Identifier, MarketId);
        }
    }
}