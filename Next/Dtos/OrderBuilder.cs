using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Dtos
{
    /// <summary>
    /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Order-entry
    /// </summary>
    public class OrderBuilder
    {
        public Account Account { get; set; }

        public Instrument Instrument { get; set; }

        /// <summary>
        /// The price limit of the order
        /// </summary>
        public decimal Price { get; set; }

        public uint Volume { get; set; }
        
        public OrderSide Side { get; set; }

        /// <summary>
        /// The currency that the instrument is traded in.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// FAK, FOK or NORMAL, NORMAL is the default if order_type is left out.
        /// </summary>
        public OrderTypeEnum Type { get; set; }

        /// <summary>
        /// Date on format YYYY-MM-DD, if left out the order is a day order. (Same behavior as if valid_date should be set to today). Can't be set when smart_order=true.
        /// </summary>
        public DateTime? ValidUntil { get; set; }

        /// <summary>
        /// 	 The visible part of an iceberg order. If left out the whole volume of the order is visible on the market
        /// </summary>
        public int OpenVolume { get; set; }

        /// <summary>
        /// Can be set to true or false. If true Nordnet Smart Order router decides which market the order is routed to.If left out smart_order defaults to false. Smart_order is only valid for day orders.
        /// </summary>
        public bool SmartOrder { get; set; }
    }
}
