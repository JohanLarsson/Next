namespace Next.FeedCommands
{
    public class SubscribeDepthArgs : SubscribeInstrumentArgsBase
    {
        public SubscribeDepthArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.Depth)
        {
        }
    }
}