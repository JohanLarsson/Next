using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
namespace Next
{
    public class PublicFeedMessageHandler
    {
        public PublicFeedMessageHandler(PublicFeed feed)
        {
            Feed = feed;
        }

        public void ReceivedMesage(string message)
        {
            
        }

        public PublicFeed Feed { get; set; }
    }
}
