namespace Next.FeedCommands
{
    public class SubscribeTradingStatusArgs : SubscribeInstrumentArgsBase
    {
        public SubscribeTradingStatusArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.TradingStatus)
        {
        }
    }
}