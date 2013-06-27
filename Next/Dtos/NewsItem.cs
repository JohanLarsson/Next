using System;

namespace Next.Dtos
{
    public class NewsItem
    {
        public DateTime Datetime { get; set; }
        public string Headline { get; set; }
        public string Itemid { get; set; }
        public int Sourceid { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("datetime: {0}, headline: {1}, itemid: {2}, sourceid: {3}, type: {4}", Datetime, Headline, Itemid, Sourceid, Type);
        }
    }
}