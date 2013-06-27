namespace Next.FeedCommands
{
    public class SubscribeIndexArgs : SubscribeArgsBase
    {
        public SubscribeIndexArgs(InstrumentDescriptor instrument) : base(instrument, SubscribeType.Index)
        {
        }
    }
}