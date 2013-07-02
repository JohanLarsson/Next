using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;

namespace NextTests.Prototypes
{
    public class CryptoPrototype
    {
        [Test]
        public void RoundTripTest()
        {
            //RSA rsa = RSA.Create();
            var rsaService = new RSACryptoServiceProvider(2048);
            //rsaService.ImportParameters(rsa.ExportParameters(true));
            string xmlString = rsaService.ToXmlString(true);
            Console.WriteLine(xmlString);
            string username = "username";
            byte[] bytes = Encoding.UTF8.GetBytes(username);
            byte[] encryptedValue = rsaService.Encrypt(bytes,false);
            Console.WriteLine(encryptedValue);
            string s = Encoding.UTF8.GetString(rsaService.Decrypt(encryptedValue,false));
            Assert.AreEqual(username,s);
        }

        [Test]
        public void ClientServerRoundtripTest()
        {
            string username = "username";
            string password = "password";
            RSAParameters publicKey = Properties.Settings.Default.TestApiInfoLocalHost.PublicKey;
            
            string encrypt = Next.NextClient.Encrypt(username, password, publicKey);
            
            // Server
            var rsaService = new RSACryptoServiceProvider();
            rsaService.FromXmlString(Properties.Settings.Default.PrivateKey);
            byte[] data = Convert.FromBase64String(encrypt);
            byte[] decrypt = rsaService.Decrypt(data, false);
            string[] strings = Encoding.UTF8.GetString(decrypt).Split(':');
            Assert.AreEqual(username, FromBase64(strings[0]));
            Assert.AreEqual(password, FromBase64(strings[1]));
        }

        private string FromBase64(string base64)
        {
            byte[] fromBase64String = Convert.FromBase64String(base64);
            string s = Encoding.UTF8.GetString(fromBase64String);
            return s;
        }
    }
}
