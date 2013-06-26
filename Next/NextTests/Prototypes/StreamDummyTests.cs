using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NextTests.Prototypes
{
    [Explicit]
    class StreamDummyTests
    {

        private string _test = "test";
        private const string _endMarker = "\n";

        [Test]
        public void WriteDirectlyToStreamTest()
        {
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(_test);
                memoryStream.Write(buffer, 0, buffer.Length);
                byte[] bytes = Encoding.UTF8.GetBytes(_endMarker);
                memoryStream.Write(bytes, 0, bytes.Length);

                Assert.AreEqual(_test + _endMarker, Read(memoryStream, true));
                Assert.AreEqual(_test + _endMarker, Read(memoryStream, false));
            }
        }

        [Test]
        public void WriteDirectlyToSslStreamTest()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var sslStream = new SslStream(memoryStream,true))
                {
                    sslStream.AuthenticateAsServer(new X509Certificate());
                    byte[] buffer = Encoding.UTF8.GetBytes(_test);
                    sslStream.Write(buffer, 0, buffer.Length);
                    byte[] bytes = Encoding.UTF8.GetBytes(_endMarker);
                    sslStream.Write(bytes, 0, bytes.Length);

                    Assert.AreEqual(_test + _endMarker, Read(sslStream, true));
                    Assert.AreEqual(_test + _endMarker, Read(sslStream, false));
                }

            }
        }

        [Test]
        public void StreamWriterTest()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, 256, true))
                {
                    writer.Write(_test);
                    writer.Write(_endMarker);
                }

                Assert.AreEqual(_test + _endMarker, Read(memoryStream, true));
                Assert.AreEqual(_test + _endMarker, Read(memoryStream, false));
            }
        }

        private string Read(Stream stream, bool lookForByteOrderMark)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream, Encoding.UTF8, lookForByteOrderMark, 256, true))
            {
                string line = reader.ReadToEnd();
                return line;
            }
        }

    }
}
