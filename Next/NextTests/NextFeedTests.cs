using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Next;
using Next.Dtos;
using Next.FeedCommands;
using NextTests.Helpers;

namespace NextTests
{
    [Explicit]
    public class NextFeedTests : NextTestsBase
    {
        private static readonly Func<NextClient, FeedInfo>[] FeedInfos =
            {
                c=>c.Session.PublicFeed,
                c=>c.Session.PrivateFeed
            };

        [Test,Explicit,TestCaseSource("FeedInfos")]
        public async Task LoginTest(Func<NextClient, FeedInfo> feedInfo)
        {
            NextClient loggedInClient = LoggedInClient;
            Console.WriteLine(feedInfo(loggedInClient));
            var data = new List<string>();
            using (var feed = new NextFeed(loggedInClient, feedInfo))
            {
                feed.ReceivedSomething += (o, e) =>
                    {
                        Console.WriteLine(e);
                        data.Add(e);
                    };
                feed.Login();
                for (int i = 0; i < 11; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.WriteLine(i);
                }

            }
            Assert.IsTrue(data.Count > 0);
            //data.ForEach(Console.WriteLine);
        }

        [TestCase("41647", 19,30,"Microsoft"),Explicit]
        public async Task SubscribeTest(string identifier, int market, int secondsToRun,string dummy)
        {
            using (NextClient loggedInClient = LoggedInClient)
            {
                //Probably need to wait for Feed to finish logging in
                var data = new List<string>();
                loggedInClient.PublicFeed.ReceivedSomething += (o, e) =>
                    {
                        Console.WriteLine(e);
                        data.Add(e);
                    };

                await loggedInClient.PublicFeed.Subscribe(new InstrumentDescriptor(market, identifier));
                for (int i = 0; i < secondsToRun; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.WriteLine(i);
                }
            }

        }

        /// <summary>
        /// Prototype to get started
        /// </summary>
        [Test, Explicit]
        public void LoginPrivate()
        {
            NextClient client = LoggedInClient;
            FeedInfo feedInfo = client.Session.PrivateFeed;
            var endPoint = new DnsEndPoint(feedInfo.Hostname, feedInfo.Port, AddressFamily.InterNetwork);
            //var endPoint = new DnsEndPoint("bajskorv.se", privateFeed.Port, AddressFamily.InterNetwork);
            string serviceName = "NEXTAPI";
            FeedCommand<LoginArgs> loginCmd = FeedCommand.Login(serviceName, client.Session.SessionKey);
            string json = loginCmd.ToJson();
            string response = string.Empty;

            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP))
                {
                    socket.Connect(endPoint);
                    socket.SendTimeout = 10000;
                    socket.ReceiveTimeout = 10000;
                    using (var sslStream = new SslStream(new NetworkStream(socket, true), true, ValidateRemoteCertificate))
                    {
                        sslStream.AuthenticateAsClient(feedInfo.Hostname);

                        var data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data);
                        sslStream.Write(Encoding.UTF8.GetBytes("\n"));

                        using (var streamreader = new StreamReader(sslStream))
                        {
                            response = streamreader.ReadLine();
                        }
                    }
                }
                Console.WriteLine("response : {0}", response);

                Assert.IsNotNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Prototype to get started
        /// </summary>
        [Test, Explicit]
        public void LoginPublic()
        {
            NextClient client = LoggedInClient;
            FeedInfo feedInfo = client.Session.PublicFeed;
            IPAddress hostAddresses = Dns.GetHostAddresses(feedInfo.Hostname).Single(x => x.AddressFamily == AddressFamily.InterNetwork);
            var endPoint = new IPEndPoint(hostAddresses, feedInfo.Port);
            string serviceName = "NEXTAPI";
            FeedCommand<LoginArgs> loginCmd = FeedCommand.Login(serviceName, client.Session.SessionKey);
            string json = loginCmd.ToJson();

            string response = string.Empty;
            try
            {
                using (var socket = new Socket(hostAddresses.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(endPoint);
                    if (feedInfo.Encrypted)
                    {
                        using (var sslStream = new SslStream(new NetworkStream(socket, false), true, ValidateRemoteCertificate))
                        {
                            sslStream.AuthenticateAsClient(feedInfo.Hostname);
                            Console.WriteLine("sslStream.CanWrite : " + sslStream.CanWrite);
                            if (sslStream.CanWrite)
                            {
                                sslStream.Write(Encoding.UTF8.GetBytes(json));
                                sslStream.Write(Encoding.UTF8.GetBytes("\n"));
                            }
                            else
                            {
                                throw new Exception("Cannot write to stream");
                            }

                            using (var reader = new StreamReader(sslStream))
                            {
                                response = reader.ReadLine();
                            }
                        }
                    }

                }

                Console.WriteLine(response);
                Assert.IsNotNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    //Console.WriteLine("The X509Chain.ChainStatus returned an array " + "of X509ChainStatus objects containing error information.");
                }
                else if (sslPolicyErrors ==
                SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    //Console.WriteLine("There was a mismatch of the name " + "on a certificate.");
                }
                else if (sslPolicyErrors ==
                SslPolicyErrors.RemoteCertificateNotAvailable)
                {
                    //Console.WriteLine("No certificate was available.");
                }
                else
                {
                    //Console.WriteLine("SSL Certificate Validation Error!");

                }
            }
            //Console.WriteLine(Environment.NewLine + "SSL Certificate Validation Error!");
            //Console.WriteLine(sslPolicyErrors.ToString());

            return false;
        }
    }
}
