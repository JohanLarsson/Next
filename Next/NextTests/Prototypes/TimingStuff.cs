using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NextTests.Helpers;
using RestSharp;

namespace NextTests.Prototypes
{
    class TimingStuff
    {
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        public void CreationsOfNClients(int n)
        {
            n++;
            RestClient[] clients = new RestClient[n];
            Stopwatch stopwatch = Stopwatch.StartNew();
            clients[0] = new RestClient(@"https://api.test.nordnet.se/next")
            {
                Authenticator = new HttpBasicAuthenticator("sessionkey", "sessionkey")
            };
            Console.WriteLine("newing first client took " + stopwatch.GetTimeString());
            stopwatch.Restart();
            for (int i = 1; i < n; i++)
            {
                clients[i] = new RestClient(@"https://api.test.nordnet.se/next")
                {
                    Authenticator = new HttpBasicAuthenticator("sessionkey", "sessionkey")
                };
            }
            Console.WriteLine("newing {0} more clients took {1}", n - 1, stopwatch.GetTimeString());
        }
    }
}
