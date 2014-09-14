using System;

namespace Next.Dtos
{
    using Newtonsoft.Json;

    public class DepthTick : InstrumentTick
    {
        public DateTime TickTimestamp { get; set; }

        public decimal Bid1 { get; set; }
       
        public int BidVolume1 { get; set; }
        
        public double Ask1 { get; set; }
        
        public int AskVolume1 { get; set; }

        public double Bid2 { get; set; }

        public int BidVolume2 { get; set; }
     
        public double Ask2 { get; set; }
        
        public int AskVolume2 { get; set; }
        
        public double Bid3 { get; set; }

        public int BidVolume3 { get; set; }
     
        public double Ask3 { get; set; }
        
        public int AskVolume3 { get; set; }
        
        public double Bid4 { get; set; }

        public int BidVolume4 { get; set; }
        
        public double Ask4 { get; set; }
        
        public int AskVolume4 { get; set; }
        
        public double Bid5 { get; set; }
        
        public int BidVolume5 { get; set; }
        
        public double Ask5 { get; set; }
        
        public int AskVolume5 { get; set; }
    }
}