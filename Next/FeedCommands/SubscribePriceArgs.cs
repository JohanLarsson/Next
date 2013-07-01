using Next.Dtos;

namespace Next.FeedCommands
{
    public class SubscribePriceArgs : SubscribeInstrumentArgsBase
    {
        public SubscribePriceArgs(InstrumentDescriptor instrument) : base(instrument,SubscribeType.Price)
        {
        }
    }
}