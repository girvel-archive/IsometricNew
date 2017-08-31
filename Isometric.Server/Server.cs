using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Isometric.Server.Common;

namespace Isometric.Server
{
    public class Server<T> : IDisposable
    {
        public EndPoint EndPoint { get; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public List<Connection<T>> Connections { get; set; } = new List<Connection<T>>();

        public Log Log { get; set; }

        public RequestManager<T> RequestManager { get; set; }

        public List<Account<T>> Accounts { get; set; } = new List<Account<T>>();

        public NewsManager<T> NewsManager { get; set; } = new NewsManager<T>();



        private readonly Socket _socket;



        /// <summary>
        /// Testing ctor
        /// </summary>
        public Server() { }



        public Server(EndPoint endPoint, RequestManager<T> requestManager, Log log)
        {
            Log = log;
            RequestManager = requestManager;

            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(EndPoint = endPoint);
        }



        /// <summary>
        /// Starts Server
        /// </summary>
        public void Start()
        {
            Log.Message("Server started");
            _socket.Listen(10);

            while (true)
            {
                Socket newSocket;
                var connection = new Connection<T>(newSocket = _socket.Accept(), this);

                connection.StartThread();
                Connections.Add(connection);
                Log.Message($"Accepted connection from {newSocket.RemoteEndPoint.ToLogString()}");
            }
        }



        public void Dispose()
        {
            _socket.Close();
        }
    }
}