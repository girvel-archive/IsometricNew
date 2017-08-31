using System;
using System.Threading;
using Isometric.Core.Time;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture()]
    public class SessionTests
    {
        [Test]
        public void StartClocks_CallsTickEveryPeriod()
        {
            // arrange
            var clocksMock = new Mock<Clocks>();
            var session = new Session {Clocks = clocksMock.Object};
            var time = new TimeSpan(0, 0, 0, 0, 50);

            // act
            var thread = session.StartClocksThread(time);
            Thread.Sleep(time + new TimeSpan(0, 0, 0, 0, 25));
            thread.Abort();

            // assert
            clocksMock.Verify(c => c.Tick(time), Times.Exactly(2));
        }
    }
}