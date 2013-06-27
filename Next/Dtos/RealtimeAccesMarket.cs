using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Dtos
{
    public class RealtimeAccesMarket
    {
        public string MarketID { get; set; }
        public int Level { get; set; }

        public override string ToString()
        {
            return string.Format("marketID: {0}, Level: {1}", MarketID, Level);
        }
    }
}
