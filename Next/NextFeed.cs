using System;
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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Next.Annotations;

    public abstract class NextFeed : IDisposable, INotifyPropertyChanged
    {
        private const string _serviceName = "NEXTAPI";
        private readonly NextClient _client;
        private readonly Func<NextClient, FeedInfo> _feedInfo;
        private readonly Socket _socket;
        private SslStream _sslStream;
        private FeedInfo _info;
        private bool _isLoggedIn;
        private DateTime _lastHeartBeatTime;

        internal NextFeed(NextClient client, Func<NextClient, FeedInfo> feedInfo)
        {
            _client = client;
            _feedInfo = feedInfo;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// For debugging, dumps the raw message
        /// </summary>
        public event EventHandler<string> WroteSomething;

        /// <summary>
        /// For debugging, dumps the raw message
        /// </summary>
        public event EventHandler<string> ReceivedSomething;

        /// <summary>
        /// When an error message is recieved
        /// </summary>
        public event EventHandler<string> ReceivedError;

        /// <summary>
        /// When an unknown message is recieved, suggested use: log it
        /// </summary>
        public event EventHandler<string> ReceivedUnknownMessage;

        public event EventHandler<ExceptionEventArgs> Exception;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime LastHeartBeatTime
        {
            get
            {
                return _lastHeartBeatTime;
            }
            set
            {
                if (value.Equals(_lastHeartBeatTime))
                {
                    return;
                }
                _lastHeartBeatTime = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
            set
            {
                if (value.Equals(_isLoggedIn))
                {
                    return;
                }
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        public FeedInfo Info
        {
            get
            {
                return _info;
            }
            set
            {
                if (Equals(value, _info))
                {
                    return;
                }
                _info = value;
                OnPropertyChanged();
            }
        }

        public async Task Login()
        {
            if (_client.Session == null)
                return;
            Info = _feedInfo(_client);
            IPAddress[] ipAddresses = await Dns.GetHostAddressesAsync(Info.Hostname);
            IPAddress hostAddresses = ipAddresses.Single(x => x.AddressFamily == AddressFamily.InterNetwork);
            var endPoint = new IPEndPoint(hostAddresses, Info.Port);
            await Task.Factory.FromAsync(_socket.BeginConnect, _socket.EndConnect, endPoint, null);
            _socket.SendTimeout = 10000;
            _socket.ReceiveTimeout = 10000;
            _sslStream = new SslStream(new NetworkStream(_socket, true), true, ValidateRemoteCertificate);
            await _sslStream.AuthenticateAsClientAsync(Info.Hostname);
            Read();
            await Write(FeedCommand.Login(_serviceName, _client.Session.SessionKey));
            IsLoggedIn = true;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async void Read()
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

        protected virtual void OnReceivedSomething(string message)
        {
            EventHandler<string> handler = ReceivedSomething;
            if (handler != null) handler(this, message);
        }

        protected virtual void OnWroteSomething(string e)
        {
            EventHandler<string> handler = WroteSomething;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnReceivedError(string e)
        {
            var handler = ReceivedError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnReceivedUnknownMessage(string e)
        {
            var handler = ReceivedUnknownMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnException(ExceptionEventArgs e)
        {
            var handler = Exception;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sslStream != null)
                {
                    _sslStream.Dispose();
                    _sslStream = null;
                }
                if (_socket != null)
                {
                    _socket.Dispose();
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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
