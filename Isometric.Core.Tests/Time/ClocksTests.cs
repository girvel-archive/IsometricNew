using System;
using System.Linq;
using Isometric.Core.Time;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.Time
{
    [TestFixture]
    public class ClocksTests
    {
        [Test]
        public void Tick_CallsTickOfEveryTimeObject()
        {
            // arrange
            var mocks = new[]
            {
                new Mock<ITimeObject>(),
                new Mock<ITimeObject>()
            };

            var clocks = new Clocks(mocks.Select(m => m.Object).ToArray());

            // act
            clocks.Tick(new TimeSpan(100));

            // assert
            foreach (var mock in mocks)
            {
                mock.Verify(o => o.Tick(new TimeSpan(100)), Times.Once);
            }
        }
    }
}