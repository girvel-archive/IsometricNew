using System;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class AreaTests
    {
        [Test]
        public void Tick_CallsTickOfBuildings()
        {
            // arrange
            var buildingMock = new Mock<Building>();
            var area = new Area(new World { AreaWidth = 2 }, new Vector())
            {
                [0, 0] = buildingMock.Object,
                [0, 1] = buildingMock.Object,
                [1, 0] = buildingMock.Object,
                [1, 1] = buildingMock.Object,
            };
            var time = new TimeSpan();

            // act
            area.Tick(time);

            // assert
            buildingMock.Verify(b => b.Tick(time), Times.Exactly(4));
        }
    }
}