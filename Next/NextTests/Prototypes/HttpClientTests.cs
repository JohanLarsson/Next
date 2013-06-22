using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NextTests.Prototypes
{
    /// <summary>
    /// Prototype to play around with HttpClient to see if thrid party is needed
    /// </summary>
    public class HttpClientTests
    {
        [Test]
        public async void TestNameTest()
        {
            var client = new HttpClient();
            client.BaseAddress =new Uri( @"https://api.test.nordnet.se/next/1");
            HttpResponseMessage response =await client.GetAsync("");
        }
    }
}
