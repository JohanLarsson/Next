using Next.Dtos;

namespace Next.FeedCommands
{
    public class SubscribeIndexArgs : SubscribeInstrumentArgsBase
    {
        public SubscribeIndexArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.Index)
        {
        }
    }
}