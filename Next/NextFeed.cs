using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Next.Dtos;

namespace Next
{
    public class NextFeed :IDisposable
    {
        private const string _serviceName = "NEXTAPI";
        private readonly NextClient _client;
        private readonly Func<NextClient, FeedInfo> _feedInfo;
        private Socket _socket;

        public NextFeed(NextClient client, Func<NextClient,FeedInfo> feedInfo )
        {
            _client = client;
            _feedInfo = feedInfo;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public void Login()
        {
            if(_client.Session==null)
                return;
            FeedInfo feedInfo = _feedInfo(_client);
            IPAddress hostAddresses = Dns.GetHostAddresses(feedInfo.Hostname).Single(x => x.AddressFamily == AddressFamily.InterNetwork);
            var endPoint = new IPEndPoint(hostAddresses, feedInfo.Port);

            
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}
