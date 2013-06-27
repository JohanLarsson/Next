using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace NextTests.Prototypes
{
    class ProxyPlayground
    {
        [Test]
        public void TestNameTest()
        {
            var client = new RestClient(@"https://api.test.nordnet.se/next/");
            IWebProxy proxy = WebRequest.DefaultWebProxy;
            proxy.Credentials = CredentialCache.DefaultCredentials;
            client.Proxy = proxy;
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("ContentType", "application/x-www-form-urlencoded");
            client.AddDefaultHeader("User-Agent","Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36");
            var restRequest = new RestRequest("login");
            IRestResponse response = client.Execute(restRequest);
            Console.Write(response.Content);
        }

        //For assistance, contact your network support team.<br><br>Your request was categorized by Blue Coat Web Filter as 'Brokerage/Trading'. <br>If you wish to question or dispute this result, please click <a href="http://sitereview.bluecoat.com/sitereview.jsp?referrer=136&url=tcp://api.test.nordnet.se:443/">here</a>.
    }
}
