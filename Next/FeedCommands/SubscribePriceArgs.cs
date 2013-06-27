namespace Next.FeedCommands
{
    public class SubscribePriceArgs : SubscribeArgsBase
    {
        public SubscribePriceArgs(InstrumentDescriptor instrument) : base(instrument,SubscribeType.Price)
        {
        }
    }
}