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
    [Explicit]
    public class RestClientExtTests
    {
        [Test]
        public void ExecuteTest()
        {
            var restClient = new RestClient(@"https://api.test.nordnet.se/next/1");
            IRestResponse<DummyServiceStatus> restResponse = restClient.Execute<DummyServiceStatus>(new RestRequest(Method.GET));
            Assert.IsInstanceOf<DummyServiceStatus>(restResponse.Data);
            Assert.IsNotNull(restResponse.Data.Timestamp);
        }

        [Test]
        public void ExecuteTaskAsyncResultTest()
        {
            var restClient = new RestClient(@"https://api.test.nordnet.se/next/1");
            Task<IRestResponse<DummyServiceStatus>> restResponse = restClient.ExecuteTaskAsync<DummyServiceStatus>(new RestRequest(Method.GET));
            IRestResponse<DummyServiceStatus> response = restResponse.Result;
            Assert.IsInstanceOf<DummyServiceStatus>(response.Data);
            Assert.IsNotNull(response.Data.Timestamp);
        }

        [Test]
        public async void ExecuteTaskAsyncAwaitTest()
        {
            var restClient = new RestClient(@"https://api.test.nordnet.se/next/1");
            Task<IRestResponse<DummyServiceStatus>> restResponse = restClient.ExecuteTaskAsync<DummyServiceStatus>(new RestRequest(Method.GET));
            IRestResponse<DummyServiceStatus> response = await restResponse;
            Assert.IsInstanceOf<DummyServiceStatus>(response.Data);
            Assert.IsNotNull(response.Data.Timestamp);
        }

        [Test]
        public void CheckOnceResultTest()
        {
            Assert.IsTrue(CheckStatus().Result);
        }

        [Test]
        public async void CheckOnceAwaitTest()
        {
            Assert.IsTrue(await CheckStatus());
        }

        [Test]
        public async Task CheckStatusTwiceAwaitTest()
        {
            Assert.IsTrue(await CheckStatus());
            Assert.IsTrue(await CheckStatus());
        }

        [Test]
        public async Task CheckStatusTwiceResultTest()
        {
            Assert.IsTrue(CheckStatus().Result); // This hangs
            Assert.IsTrue(await CheckStatus());
        }

        private async Task<bool> CheckStatus()
        {
            var restClient = new RestClient(@"https://api.test.nordnet.se/next/1");
            Task<IRestResponse<DummyServiceStatus>> restResponse = restClient.ExecuteTaskAsync<DummyServiceStatus>(new RestRequest(Method.GET));
            IRestResponse<DummyServiceStatus> response = await restResponse;
            return response.Data.SystemRunning;
        }
    }
}
