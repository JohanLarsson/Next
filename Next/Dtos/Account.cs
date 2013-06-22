using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Dtos
{
    public class Account
    {
        public object Alias { get; set; }
        public string Default { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format("Alias: {0}, Default: {1}, Id: {2}", Alias, Default, Id);
        }
    }
}
