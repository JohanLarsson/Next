namespace Next.FeedCommands
{
    public class SubscribeDepthArgs : SubscribeArgsBase
    {
        public SubscribeDepthArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.Depth)
        {
        }
    }
}