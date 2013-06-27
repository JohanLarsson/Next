using System;

namespace Next.Dtos
{
    public class TradingDay
    {
        public DateTime Date { get; set; }
        public DateTime DisplayDate { get; set; }

        public override string ToString()
        {
            return string.Format("Date: {0}, DisplayDate: {1}", Date, DisplayDate);
        }
    }
}