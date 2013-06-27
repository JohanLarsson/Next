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
        public void Login()
        {
            NextClient nextClient = new NextClient();
            //Assert.IsTrue(nextClient.Login());
        }

        [Test]
        public void Logout()
        {
            NextClient nextClient = new NextClient();
            //Assert.IsTrue(nextClient.Login());
            Console.WriteLine(nextClient.Logout());
        }
    }
}
