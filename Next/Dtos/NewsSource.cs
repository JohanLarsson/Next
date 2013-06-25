using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Dtos
{
    public class NewsSource
    {
        public string Name { get; set; }
        public string Imageurl { get; set; }
        public string Code { get; set; }
        public int Sourceid { get; set; }
        public string Level { get; set; }

        public override string ToString()
        {
            return string.Format("name: {0}, imageurl: {1}, code: {2}, sourceid: {3}, level: {4}", Name, Imageurl, Code, Sourceid, Level);
        }
    }
}
