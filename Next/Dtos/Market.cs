using System.Collections.Generic;

namespace Next.Dtos
{
    public class Market
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string MarketID { get; set; }
        public List<Ordertype> Ordertypes { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Country: {1}, MarketID: {2}, Ordertypes: {{{3}}}", Name, Country, MarketID,string.Join(", ", Ordertypes));
        }
    }
}