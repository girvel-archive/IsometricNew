using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Isometric.Server.Tests
{
    [TestFixture]
    public class RequestManagerTests
    {
        [Test]
        public void GetResponse_ReturnsCorrectResponse()
        {
            // arrange
            var manager = new RequestManager<object>(
                new Dictionary<string, ResponsePair<object>>
                {
                    ["refresh"] = new ResponsePair<object>((args, c) => new Dictionary<string, dynamic> { ["response"] = "nothing" }, ""),
                },
                null, null);

            // act
            var response = manager.GetResponse(
                new JObject { ["type"] = "refresh", ["args"] = new JObject() }.ToString(), 
#pragma warning disable 618
                new Connection<object>());
#pragma warning restore 618

            // assert
            Assert.AreEqual(JToken.Parse(response)["response"].ToString(), "nothing");
        }


        [Test]
        public void GetResponse_ThrowsInvalidRequestExceptionWhenRequestIsIncorrect()
        {
            // arrange
            var manager = new RequestManager<object>(new Dictionary<string, ResponsePair<object>>(), null, null);

            try
            {
            // act
                manager.GetResponse(
                    new JObject { ["type"] = "refresh", ["args"] = new JObject() }.ToString(),
                    null);
            }
            catch (RequestManager<object>.InvalidRequestException)
            {
            // assert
                Assert.IsTrue(true);

                return;
            }

            Assert.Fail();
        }

        [Test]
        public void GetResponse_ShouldConsiderUserGroup()
        {
            // arrange
            var manager = new RequestManager<object>(
                new Dictionary<string, ResponsePair<object>>
                {
                    ["login"] = new ResponsePair<object>(
                        (args, c) =>
                        {
                            c.Account = new Account<object>("", "", new []{"a"}, null);
                            return null;
                        }, 
                        ""),
                    ["b"] = new ResponsePair<object>(
                        (args, c) => new Dictionary<string, dynamic>(), "b")
                }, 
                (args, c) => new Dictionary<string, dynamic> {["error type"] = "permission"},
                null);

#pragma warning disable 618
            var connection = new Connection<object>();
#pragma warning restore 618

            var request = new JObject {["type"] = "b", ["args"] = null}.ToString();

            // act
            var beforeLogin = JObject.Parse(manager.GetResponse(request, connection));
            manager.GetResponse(new JObject { ["type"] = "login", ["args"] = null }.ToString(), connection);
            var afterLogin = JObject.Parse(manager.GetResponse(request, connection));

            // assert
            Assert.IsTrue(beforeLogin["error type"].ToString() == "permission");
            Assert.IsTrue(beforeLogin["error type"].ToString() == "permission");
        }
    }
}