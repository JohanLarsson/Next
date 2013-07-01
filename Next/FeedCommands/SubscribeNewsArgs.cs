using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Dtos;

namespace Next.FeedCommands
{
    /// <summary>
    /// https://api.test.nordnet.se/projects/api/wiki/Feed_API_documentation#Subgroup-Subscribe-request
    /// </summary>
    public class SubscribeNewsArgs
    {


        public SubscribeNewsArgs(int newsSourceId, bool delay=false)
        {
            this.s = newsSourceId;
            this.delay = delay;
        }
        public SubscribeNewsArgs(NewsSource newsSource, bool delay=false)
        {
            this.s = newsSource.Sourceid;
            this.delay = delay;
        }
        private string _t =SubscribeType.News;
        public string t
        {
            get { return _t; }
            set { _t = value; }
        }

        public int s { get; set; }
        public bool delay { get; set; }

        public override string ToString()
        {
            return string.Format("Type(t): {0} newsSource.Sourceid(s): {1}, delay: {2}",t, s, delay );
        }
    }
}
