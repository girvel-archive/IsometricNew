using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;

namespace Isometric.Server.Tests
{
    [TestFixture]
    public class ServerTests
    {
        [Test]
        public void Start_AcceptsConnections()
        {
            // arrange
            using (var server = new Server<object>(NetHelper.Current.GetIpEndPoint(), null, new Log()))
            using (var socket1 = new Socket(SocketType.Stream, ProtocolType.Tcp))
            using (var socket2 = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket1.Bind(NetHelper.Current.GetIpEndPoint());
                socket2.Bind(NetHelper.Current.GetIpEndPoint());

                // act
                new Thread(server.Start).Start();

                socket1.Connect(server.EndPoint);
                socket2.Connect(server.EndPoint);

                // assert
                Assert.IsTrue(socket1.Connected, "socket1 can not connect to server");
                Assert.IsTrue(socket2.Connected, "socket2 can not connect to server");
            }
        }


        [Test]
        public void Start_CreatesConnections()
        {
            // arrange
            using (var server = new Server<object>(NetHelper.Current.GetIpEndPoint(), null, new Log()))
            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(NetHelper.Current.GetIpEndPoint());

                // act
                new Thread(server.Start).Start();

                // assert
                socket.Connect(server.EndPoint);
                Thread.Sleep(20);

                Assert.AreEqual(1, server.Connections.Count);
            }
        }
    }
}