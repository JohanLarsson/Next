using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;

namespace NextTests.Mocks
{
    /// <summary>
    /// This is kinda moot but the reason is to try to learn
    /// </summary>
    [Explicit]
    public class MockServiceTests
    {
        private CancellationTokenSource _cts;
        private Task _serverTask;

        [SetUp]
        public void SetUp()
        {
            _cts = new CancellationTokenSource();
            _serverTask = Task.Factory.StartNew(() =>
                {
                    //cts.Token.ThrowIfCancellationRequested();
                    using (MockService mockService = new MockService())
                    {
                    }
                }, _cts.Token);
        }

        [Test]
        public async Task VanillaTest()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Properties.Settings.Default.TestApiInfoLocalHost.BaseUrl);
                Task<HttpResponseMessage> get = client.GetAsync("Test");
                using (HttpResponseMessage response = get.Result)
                {
                    HttpContent httpContent = response.Content;
                    var s = await httpContent.ReadAsStringAsync();
                    Console.WriteLine("Client received: " + s);
                }
            }

            try
            {
                _cts.Cancel();
                _serverTask.Wait(_cts.Token);
            }
            catch (Exception)
            {
            }

        }

        [Test]
        public async Task LoginTest()
        {
            ApiInfo apiInfo = Properties.Settings.Default.TestApiInfoLocalHost;
            var nextClient = new NextClient(apiInfo);
            bool login =await  nextClient.Login(MockService.UserName, MockService.Password);
        }
    }
}