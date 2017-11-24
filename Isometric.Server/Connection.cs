using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Isometric.Server.Common;

namespace Isometric.Server
{
    public class Connection<T>
    {
        public Server<T> Server { get; set; }

        public Thread Thread { get; set; }

        public Account<T> Account
        {
            get { return _account; }
            set
            {
                _account = value;

                if (Account != null)
                {
                    Server?.NewsManager?.BeginNewsRegistration(Account.ExternalData);
                }
            }
        }


        private readonly Socket _socket;

        private readonly Log _log;



        [Obsolete("Testing ctor")]
        public Connection() { } 

        public Connection(Socket socket, Server<T> server)
        {
            _socket = socket;
            Server = server;
            _log = Server.Log;
        }



        public void StartThread()
        {
            Thread = new Thread(Start);
            Thread.Start();
        }

        private readonly object _connectionLock = new object();
        private Account<T> _account;

        public void Start()
        {
            lock (_connectionLock)
            {
                while (true)
                {
                    string receivedData = "-";
                    try
                    {
                        string response;
                        Send(response = Server.RequestManager.GetResponse(receivedData = Receive(), this));
                        _log.Message(">> " + response);
                    }
                    catch (SocketException)
                    {
                        Close();
                    }
#if !DEBUG
                    catch (Exception ex)
                    {
                        _log.Message(
                            $"Catched {ex.GetType()}: received request " +
                            $"from {_socket.RemoteEndPoint.ToLogString()} is wrong ({ex.Message}).\n\n" +
                            $"Request: {receivedData}");

                        if (receivedData == "")
                        {
                            Close();
                        }
                    }
#endif
                }
            }
        }
        
        public void Close()
        {
            _log.Message($"Closed connection to {_socket.RemoteEndPoint?.ToLogString() ?? "-"}");

            if (Account != null)
            {
                Server.NewsManager.EndNewsRegistration(Account.ExternalData);
            }

            _socket.Close();
            Server.Connections.Remove(this);
            Thread?.Abort();
        }



        private void Send(string request)
        {
            _socket.Send(Server.Encoding.GetBytes(request));
        }

        private string Receive()
        {
            var currentStringBuilder = new StringBuilder();
            var receivedData = new byte[4096];

            do
            {
                var bytes = _socket.Receive(receivedData);

                currentStringBuilder.Append(Server.Encoding.GetString(receivedData, 0, bytes));
            }
            while (_socket.Available > 0);
            
            return currentStringBuilder.ToString();
        }
    }
}