using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Dtos
{
    public class AccountSummary
    {
        public string OwnCapitalMorning { get; set; }
        public string AccountCurrency { get; set; }
        public string OwnCapital { get; set; }
        public string FutureSum { get; set; }
        public string ForwardSum { get; set; }
        public string Collateral { get; set; }
        public string TradingPower { get; set; }
        public string Interest { get; set; }
        public string PawnValue { get; set; }
        public string AccountSum { get; set; }
        public string LoanLimit { get; set; }
        public string FullMarketvalue { get; set; }
    }
}
