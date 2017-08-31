using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Isometric.Server.Tests
{
    public class NetHelper
    {
        public static NetHelper Current => _current ?? (_current = new NetHelper());
        private static NetHelper _current;



        private int _nextPort = 8500; 

        private NetHelper() { }



        public IPEndPoint GetIpEndPoint() => new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], _nextPort++);

        public Socket GetNewBindedSocket()
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(GetIpEndPoint());
            return socket;
        }

        public void GetNewSocketPair(out Socket client, out Socket server)
        {
            var _client = new Socket(SocketType.Stream, ProtocolType.Tcp);

            using (var serverSocket = Current.GetNewBindedSocket())
            {
                serverSocket.Listen(1);

                var connectTask = new Task(() =>
                {
                    _client.Connect(serverSocket.LocalEndPoint);
                });
                connectTask.Start();

                server = serverSocket.Accept();
                connectTask.Wait();

                client = _client;
            }
        }
    }
}