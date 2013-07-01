using Next.Dtos;

namespace Next.FeedCommands
{
    public class SubscribeTradeArgs : SubscribeInstrumentArgsBase
    {
        public SubscribeTradeArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.Trade)
        {
        }
    }
}