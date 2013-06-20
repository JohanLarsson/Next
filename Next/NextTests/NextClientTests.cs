using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using Next;
using RestSharp;
using NextTests.Helpers;

namespace NextTests
{
    [Explicit]
    public class NextClientTests
    {
        [Test]
        public void CtorTest()
        {
            int n = 10000;
            NextClient[] nextClients = new NextClient[n];
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                nextClients[i] = new NextClient();
            }
            Console.WriteLine(stopwatch.GetTimeString());
        }

        [Test]
        public async void LoginTest()
        {
            NextClient nextClient = new NextClient();
            Assert.IsTrue(await nextClient.Login(Credentials.Username, Credentials.Password));
        }

        [Test]
        public async void LoginFailTest()
        {
            NextClient nextClient = new NextClient();
            Assert.IsFalse(await nextClient.Login("Incorrect", "Credentials"));
        }

        [Test]
        public async void LogoutTest()
        {
            Assert.IsTrue(await (await LoggedInClient()).Logout());
        }

        [Test]
        public async void LogoutWhenNotLoggedInTest()
        {
            NextClient nextClient = new NextClient();
            Assert.IsTrue(await nextClient.Logout());
        }

        [Test]
        public async void TouchTestTest()
        {
            Assert.IsTrue(await (await LoggedInClient()).Touch());
        }

        private async Task<NextClient> LoggedInClient()
        {
            //get
            //{
                NextClient nextClient = new NextClient();
                var login = await nextClient.Login(Credentials.Username, Credentials.Password);
                Assert.IsTrue(login);
                return nextClient;
            //}
        }

        public Credentials Credentials { get { return Credentials.Load(Properties.Resources.CredentialsPath); } }

    }
}
