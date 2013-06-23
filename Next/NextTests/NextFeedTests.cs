using System;
using System.Collections.Generic;
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
    public class NextFeedTests : NextTestsBase
    {
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
                        using (var writer = new StreamWriter(sslStream, Encoding.UTF8, socket.SendBufferSize, true))
                        {
                            writer.WriteLine(json);
                            writer.Write("\n");
                            writer.Flush();
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            using (var streamReader = new StreamReader(sslStream, Encoding.UTF8, false, socket.ReceiveBufferSize, true))
                            {
                                string text = streamReader.ReadToEnd();
                                Console.WriteLine("{0} {1}", i, text);
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }

                    }
                }
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
            try
            {
                using (var socket = new Socket(hostAddresses.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(endPoint);
                    socket.NoDelay = true;
                    socket.SendTimeout = 2000;
                    socket.ReceiveTimeout = 1000;
                    if (feedInfo.Encrypted)
                    {
                        using (var sslStream = new SslStream(new NetworkStream(socket, false), true, ValidateRemoteCertificate))
                        {
                            sslStream.AuthenticateAsClient(feedInfo.Hostname);
                            Console.WriteLine("sslStream.CanWrite : " + sslStream.CanWrite);
                            if (sslStream.CanWrite)
                            {
                                using (var writer = new StreamWriter(sslStream, Encoding.UTF8, socket.SendBufferSize, true))
                                {
                                    writer.Write(json);
                                    writer.Write("\n");
                                    writer.Flush();
                                }
                            }
                            else
                            {
                                throw new Exception("Cannot write to stream");
                            }

                            for (int i = 0; i < 40; i++)
                            {
                                if (sslStream.CanRead)
                                {
                                    using (var streamReader = new StreamReader(sslStream, Encoding.UTF8, false, socket.ReceiveBufferSize, true))
                                    {
                                        Console.Write(i +" ");
                                        while (streamReader.Peek() >= 0)
                                        {
                                            Console.Write((char)streamReader.Read());
                                        }
                                        Console.WriteLine();
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot read from stream");
                                }

                                Thread.Sleep(TimeSpan.FromSeconds(1));
                            }

                        }
                    }

                }
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
