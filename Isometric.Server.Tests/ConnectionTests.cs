using System.Net.Sockets;
using System.Threading;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Isometric.Server.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        [Test]
        public void Start_ReceivesAndSendsRequests()
        {
            // TODO use NetHelper.GetIpEndPoint

            // arrange
            Socket clientSocket, connectionSocket;
            NetHelper.Current.GetNewSocketPair(out clientSocket, out connectionSocket);

            using (clientSocket)
            using (connectionSocket)
            {
                var requestManagerMock = new Mock<RequestManager<object>>();
                requestManagerMock.Setup(
                    m => m.GetResponse(
                        It.IsAny<string>(),
                        It.IsAny<Connection<object>>()))
                    .Returns(new JObject { ["response"] = "nothing" }.ToString());

                var server = new Server<object>(
                    connectionSocket.LocalEndPoint,
                    requestManagerMock.Object,
                    new Log());

                var connection = new Connection<object>(connectionSocket, server);

                // act
                connection.StartThread();
                clientSocket.Send(server.Encoding.GetBytes(new JObject().ToString()));

                var response = new byte[256];
                clientSocket.Receive(response);
                connection.Close();

                // assert
                Assert.AreEqual(
                    new JObject { ["response"] = "nothing" }.ToString(),
                    server.Encoding.GetString(response).Replace("\0", ""));
            }
        }


        
        [Test]
        public void Start_WritesToLogWhenReceivedDataIsWrong()
        {
            // arrange
            var logMock = new Mock<Log>();
            logMock.Setup(l => l.Message(It.IsAny<string>()));

            Socket clientSocket, connectionSocket;
            NetHelper.Current.GetNewSocketPair(out connectionSocket, out clientSocket);
            
            using (connectionSocket)
            using (clientSocket)
            {
                var server = new Server<object>
                {
                    Log = logMock.Object,
                    RequestManager = Mock.Of<RequestManager<object>>(
                        m => m.GetResponse(It.IsAny<string>(), It.IsAny<Connection<object>>()) == "")
                };
                var connection = new Connection<object>(connectionSocket, server);

                // act
                connection.StartThread();
                clientSocket.Send(server.Encoding.GetBytes("fff"));
                Thread.Sleep(100);

                connection.Close();

                // assert
                logMock.Verify(
                    l => l.Message(It.IsAny<string>()),
                    Times.Once,
                    "connection didn't write to log");
            }
        }
    }
}