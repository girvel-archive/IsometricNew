using System;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class WorldTests
    {
        [Test]
        public void TryUpgrade_ReturnsUpgradePossibility()
        {
            // arrange
            var to = new Building { Price = new DefaultResources() };
            var from = new Building
            {
                Upgrades = new[] { to },
                Owner = new Player { Resources = new DefaultResources() },
                Finished = true,
            };

            var area = new Area(new World { AreaWidth = 1 }, new Vector()) { [0, 0] = from };
            area.World.SetArea(new Vector(), area);

            // act
            var result = area.World.TryUpgrade(new Vector(0, 0), to);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TryUpgrade_ChangesPrototype()
        {
            // arrange
            var to = new Building { Price = new DefaultResources() };
            var from = new Building
            {
                Upgrades = new[] { to },
                Owner = new Player { Resources = new DefaultResources() },
                Finished = true,
            };

            var area = new Area(new World { AreaWidth = 1 }, new Vector()) { [0, 0] = from };
            area.World.SetArea(new Vector(), area);

            // act
            area.World.TryUpgrade(new Vector(0, 0), to);

            // assert
            Assert.IsTrue(area[0, 0].Prototype == to);
        }

        [Test]
        public void Tick_CallsTickOfAllNotNullAreas()
        {
            // arrange
            var world = new World {Landscape = new Area[2, 2], AreaWidth = 10};
            var areaMock = new Mock<Area>();
            var time = new TimeSpan(1);

            world.Landscape[0, 0] = areaMock.Object;

            // act
            world.Tick(time);

            // assert
            areaMock.Verify(a => areaMock.Object.Tick(time), Times.Once);
        }

        [Test]
        public void TryUpgrade_DecreasesOwnerResources()
        {
            // arrange
            var to = new Building { Price = new DefaultResources(100) };
            var from = new Building
            {
                Upgrades = new[] { to },
                Owner = new Player { Resources = new DefaultResources(150) },
                Finished = true,
            };

            var area = new Area(new World { AreaWidth = 1 }, new Vector()) { [0, 0] = from };
            area.World.SetArea(new Vector(), area);

            // act
            area.World.TryUpgrade(new Vector(0, 0), to);

            // assert
            Assert.AreEqual(50, area[0, 0].Owner.Resources[0]);
        }
    }
}