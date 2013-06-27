namespace Next.FeedCommands
{
    public class SubscribeArgsBase 
    {
        public SubscribeArgsBase(InstrumentDescriptor instrument, string type)
        {
            m = instrument.MarketId;
            i = instrument.Identifier;
            t = type;
        }

        public int m { get; set; }
        public string i { get; set; }
        public string t { get; private set; }
    }
}