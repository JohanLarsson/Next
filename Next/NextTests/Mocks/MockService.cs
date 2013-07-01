using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Policy;
using NUnit.Framework;

namespace NextTests.Mocks
{
    public class MockService :IDisposable
    {
        private HttpListener _listener;

        public MockService()
        {
            _listener = new HttpListener
                {
                    
                    //AuthenticationSchemes = AuthenticationSchemes.Basic
                };
            _listener.Start();
            HttpListenerContext context = _listener.GetContext();
            HttpListenerRequest httpListenerRequest = context.Request;
        }

        public void Dispose()
        {
            if(_listener!=null)
                _listener.Stop();
        }
    }

    /// <summary>
    /// This is kinda moot but the reason is to try to learn
    /// </summary>
    public class MockServiceTests
    {
        [Test]
        public void TestNameTest()
        {
            MockService mockService = new MockService();
            HttpClient client = new HttpClient();
            client.BaseAddress =new Uri( @"http://localhost");

        }
    }
}
