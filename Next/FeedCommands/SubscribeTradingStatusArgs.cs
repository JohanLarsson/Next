namespace Next.FeedCommands
{
    public class SubscribeTradingStatusArgs : SubscribeArgsBase
    {
        public SubscribeTradingStatusArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.TradingStatus)
        {
        }
    }
}