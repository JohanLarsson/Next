namespace Next.FeedCommands
{
    public class SubscribeTradeArgs : SubscribeArgsBase
    {
        public SubscribeTradeArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.Trade)
        {
        }
    }
}