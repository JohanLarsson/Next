using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Next.Dtos;
using Next.FeedCommands;

namespace Next
{
    public class NextFeed : IDisposable
    {
        private const string _serviceName = "NEXTAPI";
        private readonly NextClient _client;
        private readonly Func<NextClient, FeedInfo> _feedInfo;
        private readonly Socket _socket;
        private SslStream _sslStream;

        internal NextFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo)
        {
            _client = client;
            _feedInfo = feedInfo;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Login()
        {
            if (_client.Session == null)
                return;
            FeedInfo feedInfo = _feedInfo(_client);
            IPAddress[] ipAddresses = await Dns.GetHostAddressesAsync(feedInfo.Hostname);
            IPAddress hostAddresses = ipAddresses.Single(x => x.AddressFamily == AddressFamily.InterNetwork);
            var endPoint = new IPEndPoint(hostAddresses, feedInfo.Port);
            await Task.Factory.FromAsync(_socket.BeginConnect, _socket.EndConnect, endPoint, null);
            _socket.SendTimeout = 10000;
            _socket.ReceiveTimeout = 10000;
            _sslStream = new SslStream(new NetworkStream(_socket, true), true, ValidateRemoteCertificate);
            await _sslStream.AuthenticateAsClientAsync(feedInfo.Hostname);
            Read();
            await Write(FeedCommand.Login(_serviceName, _client.Session.SessionKey));
        }

        public async Task Write<T>(FeedCommand<T> command)
        {
            string json = command.ToJson();
            const string endMarker = "\n";
            //using (var writer = new StreamWriter(_sslStream, new UTF8Encoding(false, true), _socket.SendBufferSize, true))
            //{
            //    writer.Write(json);
            //    writer.Write(endMarker);
            //    writer.Flush();
            //}
            byte[] buffer = Encoding.UTF8.GetBytes(json + endMarker);

            await _sslStream.WriteAsync(buffer, 0, buffer.Length);
            OnWroteSomething(json);
            //_sslStream.Write(Encoding.UTF8.GetBytes(endMarker));
        }

        public async Task Read()
        {
            using (var reader = new StreamReader(_sslStream, new UTF8Encoding(false, true), false, _socket.ReceiveBufferSize, true))
            {
                while (_sslStream.CanRead)
                {
                    string line = await reader.ReadLineAsync();
                    OnReceivedSomething(line);
                }
            }
        }

        public event EventHandler<string> ReceivedSomething;

        protected virtual void OnReceivedSomething(string e)
        {
            EventHandler<string> handler = ReceivedSomething;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<string> WroteSomething;

        protected virtual void OnWroteSomething(string e)
        {
            EventHandler<string> handler = WroteSomething;
            if (handler != null) handler(this, e);
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

        public void Dispose()
        {
            _sslStream.Dispose();
            _socket.Dispose();
        }
    }
}
