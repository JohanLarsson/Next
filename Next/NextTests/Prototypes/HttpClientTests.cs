using System;
using System.Collections.Generic;
using System.IO;
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
    [Explicit]
    public class HttpClientTests
    {
        [Test, Explicit]
        public async void TestNameTest()
        {
            var client = new HttpClient();
            client.BaseAddress =new Uri( @"https://api.test.nordnet.se/next/1");
            HttpResponseMessage response =await client.GetAsync("");
        }

        [Test]
        public async Task IsinTest()
        {
            var client = new HttpClient();
            //client.BaseAddress =new Uri( @"http://www.isin.org/isin-database/");
            //var content = new System.Net.Http.h
            //content.Add(new StringContent("reportFilter[dt_intfc5106d8b0b824c][EID]:dt_intfc5106d8b0b824c"));
            //content.Add(new StringContent("reportFilter[dt_intfc5106d8b0b824c][_keywords]:US9843321061"));
            //content.Add(new StringContent("search_search:Search"));
            using (HttpResponseMessage response = await client.GetAsync(@"http://www.isin.org/isin-database/isin-view/?isin=US9843321061"))
            {
                string data = await response.Content.ReadAsStringAsync();

            }
            //using (HttpResponseMessage data = await client.GetAsync(""))
            //{
            //    string readAsStringAsync =await data.Content.ReadAsStringAsync();
            //}
        }
    }
}
