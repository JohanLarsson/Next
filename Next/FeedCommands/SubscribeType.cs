namespace Next.FeedCommands
{
    public static class SubscribeType
    {
        public static string Price = "price";
        public static string Depth = "depth";
        public static string Trade = "trade";
        public static string Index = "index";
        public static string TradingStatus = "trading_status";

        public static string[] All = new[] {Price, Depth, Trade, Index, TradingStatus};
    }
}