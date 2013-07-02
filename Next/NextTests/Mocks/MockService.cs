using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using Next.Dtos;
using RestSharp.Contrib;

namespace NextTests.Mocks
{
    public class MockService : IDisposable
    {
        private HttpListener _listener;
        public const string UserName = "Username";
        public const string Password = "Password";
        public const string SessionKey = "SessionKey";
        public MockService()
        {
            _listener = new HttpListener
                {

                    //AuthenticationSchemes = AuthenticationSchemes.Basic
                };
            string baseAdress = @"http://localhost/next/1/";
            _listener.Prefixes.Add(baseAdress);
            _listener.Start();
            while (_listener.IsListening)
            {
                var context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                if (request.HttpMethod == "POST" && request.Url.LocalPath == "/next/1/login")
                    Login(context);
                //string body = new StreamReader(context.Request.InputStream).ReadToEnd();

                //Console.WriteLine("Server received: " + context.Request.Url);
                //context.Response.StatusCode = 200;
                //byte[] b = Encoding.UTF8.GetBytes("ACK");
                //context.Response.ContentLength64 = b.Length;

                //var output = context.Response.OutputStream;
                //output.Write(b, 0, b.Length);
                context.Response.Close();
            }
        }

        public void Login(HttpListenerContext context)
        {
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.FromXmlString(Properties.Settings.Default.PrivateKey);
            string body;
            HttpListenerRequest request = context.Request;
            using (var reader = new StreamReader(request.InputStream,new UTF8Encoding(false),true))
            {
                body = reader.ReadToEnd();
            }
            NameValueCollection nameValueCollection = request.QueryString;
            string[] pairs = body.Split('&');
            var valueCollection = new NameValueCollection();
            foreach (var pair in pairs)
            {
                string[] split = pair.Split('=');
                valueCollection.Add(split[0],split[1]);
            }
            if (valueCollection["service"] != "NEXTAPI")
            {
                byte[] buffer = Encoding.UTF8.GetBytes("FAIL");
                context.Response.OutputStream.Write(buffer,0,buffer.Length);
                return;
            }
            string encrypted = valueCollection["auth"];

            byte[] fromBase64String = Convert.FromBase64String(HttpUtility.UrlDecode( encrypted));
            byte[] decrypt = rsaCryptoServiceProvider.Decrypt(fromBase64String, false);
            string[] strings = Encoding.UTF8.GetString(decrypt).Split(':');
            if (strings[0] == UserName && strings[1] == Password)
            {
                var sessionInfo = new SessionInfo
                    {
                        Country = "SE", 
                        Environment = "test",
                        ExpiresIn = 300, 
                        PrivateFeed = new FeedInfo
                            {
                                Encrypted = true, Hostname = @"localhost/priv", Port = 443
                            },
                        PublicFeed = new FeedInfo
                            {
                                Encrypted = true, Hostname = @"localhost/pub", Port = 443
                            },
                        SessionKey = SessionKey
                    };
                string serialize = new RestSharp.Serializers.JsonSerializer().Serialize(sessionInfo);
                using (var writer = new StreamWriter(context.Response.OutputStream, new UTF8Encoding(false, true)))
                {
                    writer.Write(serialize);
                }
                return;
            }
            byte[] buffer1 = Encoding.UTF8.GetBytes("FAIL");
            context.Response.OutputStream.Write(buffer1, 0, buffer1.Length);
            return;
        }

        public void Dispose()
        {
            if (_listener != null)
                _listener.Stop();
        }
    }
}
