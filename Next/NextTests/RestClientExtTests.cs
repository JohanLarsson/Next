using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Next;

namespace NextTests
{
    class RestClientExtTests
    {
        [Test]
        public void TestNameTest()
        {
            var restClient = new RestClient(@"https://api.test.nordnet.se/next/1");
            IRestResponse<DummyServiceStatus> restResponse = restClient.Execute<DummyServiceStatus>(new RestRequest(Method.GET));
            Assert.IsInstanceOf<DummyServiceStatus>(restResponse.Data);
            Assert.IsNotNull(restResponse.Data.timestamp);
        }

        [Test]
        public void TaskAsyncTest()
        {
            var restClient = new RestClient(@"https://api.test.nordnet.se/next/1");
            Task<IRestResponse<DummyServiceStatus>> restResponse = restClient.ExecuteTaskAsync<DummyServiceStatus>(new RestRequest(Method.GET));
            IRestResponse<DummyServiceStatus> response = restResponse.Result;
            Assert.IsInstanceOf<DummyServiceStatus>(response.Data);
            Assert.IsNotNull(response.Data.timestamp);
        }
    }

    public class DummyServiceStatus
    {
        public string message { get; set; }
        public bool valid_version { get; set; }
        public bool system_running { get; set; }
        public bool skip_phrase { get; set; }
        public long timestamp { get; set; }
    }
}
