using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Next;
using NextTests.Helpers;

namespace NextTests.Prototypes
{
    [Explicit]
    class DumpXml
    {
        [Test]
        public void CreateCredentials()
        {
            Credentials credentials = new Credentials { Username = "user", Password = "pw" };
            credentials.Save(Properties.Resources.CredentialsPath);
        }

        [Test]
        public void CreateApiInfoTest()
        {
            string testApiKey = @"<?xml version=""1.0""?>
<RSAParameters xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<Exponent>AQAB</Exponent>
<Modulus>5td/64fAicX2r8sN6RP3mfHf2bcwvTzmHrLcjJbU85gLROL+IXclrjWsluqyt5xtc/TCwMTfC/NcRVIAvfZdt+OPdDoO0rJYIY3hOGBwLQJeLRfruM8dhVD+/Kpu8yKzKOcRdne2hBb/mpkVtIl5avJPFZ6AQbICpOC8kEfI1DHrfgT18fBswt85deILBTxVUIXsXdG1ljFAQ/lJd/62J74vayQJq6l2DT663QB8nLEILUKEt/hQAJGU3VT4APSfT+5bkClfRb9+kNT7RXT/pNCctbBTKujr3tmkrdUZiQiJZdl/O7LhI99nCe6uyJ+la9jNPOuK5z6v72cXenmKZw==</Modulus>
</RSAParameters>";
            var testApiInfo = new ApiInfo
                {
                    Host = @"https://api.test.nordnet.se",
                    Path = @"next",
                    Version = "1",
                    PublicKey = Deserialize<RSAParameters>(testApiKey)
                };
            Dump(testApiInfo);
        }

        public static void Dump<T>(T value)
        {
            var serializer = new XmlSerializer(value.GetType());
            var sb = new StringBuilder();
            using (var writer = new XmlTextWriter(new StringWriter(sb)))
            {
                serializer.Serialize(writer, value);
                Console.WriteLine(value.GetType().FullName);
                Console.WriteLine();
                Console.WriteLine(sb);
            }
        }

        public static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new XmlTextReader(new StringReader(xml)))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

    }
}
