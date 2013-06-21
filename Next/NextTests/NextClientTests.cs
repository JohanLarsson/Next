using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            RestClient[] clients = new RestClient[n];
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                clients[i] = new RestClient(@"https://api.test.nordnet.se/next")
                    {
                        Authenticator = new HttpBasicAuthenticator("sessionkey", "sessionkey")
                    };

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
            Assert.IsTrue(await LoggedInClient.Logout());
        }

        [Test]
        public async void LogoutWhenNotLoggedInTest()
        {
            NextClient nextClient = new NextClient();
            Assert.IsTrue(await nextClient.Logout());
        }

        [Test]
        public async void TouchTest()
        {
            //Assert.IsTrue(await LoggedInClient.Touch());
            Assert.IsTrue(await (await GetLoggedInClient()).Touch());
        }

        private NextClient LoggedInClient
        {
            get
            {
                NextClient nextClient = new NextClient();
                Assert.IsTrue( nextClient.Login(Credentials.Username, Credentials.Password).Result);
                return nextClient;
            }
        }

        /// <summary>
        /// This works but produces butt ugly syntax:
        /// Assert.IsTrue(await (await GetLoggedInClient()).Touch());
        /// </summary>
        /// <returns></returns>
        private async Task<NextClient> GetLoggedInClient()
        {
            NextClient nextClient = new NextClient();
            Assert.IsTrue(await nextClient.Login(Credentials.Username, Credentials.Password));
            return nextClient;
        }

        public Credentials Credentials { get { return Credentials.Load(Properties.Resources.CredentialsPath); } }

    }
}
