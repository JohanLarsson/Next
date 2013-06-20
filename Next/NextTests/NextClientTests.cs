using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using Next;
using RestSharp;

namespace NextTests
{
    public class NextClientTests
    {
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
            NextClient nextClient = new NextClient();
            Assert.IsTrue(await nextClient.Login(Credentials.Username, Credentials.Password));
            Assert.IsTrue(await nextClient.Logout());
        }

        public Credentials Credentials { get { return Credentials.Load(Properties.Resources.CredentialsPath); } }

    }
}
