using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using Next.Dtos;
using HttpUtility = RestSharp.Contrib.HttpUtility;
using RestSharp.Extensions;

namespace NextTests.Mocks
{
    /// <summary>
    /// Wrote this, the mother of all overmocks for fun
    /// This requires Visual Studio to be run as admin
    /// </summary>
    public class MockService : IDisposable
    {
        private readonly HttpListener _listener;
        public const string UserName = "Username";
        public const string Password = "Password";
        public const string SessionKey = "SessionKey";
        public MockService()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(Properties.Settings.Default.TestApiInfoLocalHost.BaseUrl + "/");
            _listener.Start();
            while (_listener != null && _listener.IsListening)
            {
                var context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                if (request.IsPost() && request.Url.LocalPath == "/next/1/login")
                    Login(context);
                context.Response.Close();
            }
        }

        public void Login(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            NameValueCollection nvc = request.GetNameValueCollection();
            if (nvc["service"] != "NEXTAPI")
            {
                context.Response.Write("Fail");
                return;
            }

            string encrypted = nvc["auth"];
            var rsaService = new RSACryptoServiceProvider();
            rsaService.FromXmlString(Properties.Settings.Default.PrivateKey);
            byte[] data = Convert.FromBase64String(encrypted);
            byte[] decrypt = rsaService.Decrypt(data, false);
            string[] strings = Encoding.UTF8.GetString(decrypt)
                                       .Split(':')
                                       .Select(s => Encoding.UTF8.GetString(Convert.FromBase64String(s)))
                                       .ToArray();

            if (strings[0] == UserName && strings[1] == Password)
            {
                var sessionInfo = new SessionInfo
                    {
                        Country = "SE",
                        Environment = "test",
                        ExpiresIn = 300,
                        PrivateFeed = new FeedInfo
                            {
                                Encrypted = true,
                                Hostname = @"localhost/priv",
                                Port = 443
                            },
                        PublicFeed = new FeedInfo
                            {
                                Encrypted = true,
                                Hostname = @"localhost/pub",
                                Port = 443
                            },
                        SessionKey = SessionKey
                    };
                context.Response.Write(sessionInfo);
                return;
            }
            context.Response.Write("Fail");
            return;
        }

        public void Dispose()
        {
            if (_listener != null)
                _listener.Stop();
        }
    }
}
