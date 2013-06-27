using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Dtos
{
    public class Ledger
    {
        public string AccountSumAcc { get; set; }
        public string AccIntCred { get; set; }
        public string Currency { get; set; }
        public string AccIntDeb { get; set; }
        public string AccountSum { get; set; }

        public override string ToString()
        {
            return string.Format("AccountSumAcc: {0}, AccIntCred: {1}, Currency: {2}, AccIntDeb: {3}, AccountSum: {4}", AccountSumAcc, AccIntCred, Currency, AccIntDeb, AccountSum);
        }
    }
}
