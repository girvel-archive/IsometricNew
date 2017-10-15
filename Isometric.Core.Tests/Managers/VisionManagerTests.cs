using System.Collections.Generic;
using Isometric.Core.Managers;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.Managers
{
    [TestFixture]
    public class VisionManagerTests
    {
        [Test]
        public void IsAreaOpened_ReturnsTrueWhenAreaIsMainPlayersOne()
        {
            // arrange
            var manager = new VisionManager();
            var area = new Area();
            var player = new Player {Area = area};

            // act
            var result = manager.IsAreaOpened(player, area);

            // assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsVisible_ReturnsTrueIfCellIsInVisionRangeOfBuilding()
        {
            // arrange
            var manager = new VisionManager();

            var player = Mock.Of<Player>();
            var world = new Mock<World>();
            world
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns((Vector v) => v == new Vector(0, 0) ? new Building {Owner = player} : null);
            
            // act
            var result = manager.IsVisible(world.Object, player, new Vector(2, 2));
            
            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void GetVision_ReturnsBuildingsInVision()
        {
            // arrange
            var buildingMocks = new[]
            {
                Mock.Of<Building>(b => b.Position == new Vector(0, 0)),
                Mock.Of<Building>(b => b.Position == new Vector(1, 4)),
                Mock.Of<Building>(b => b.Position == new Vector(1, 0)),
                Mock.Of<Building>(b => b.Position == new Vector(2, 0)),
                Mock.Of<Building>(b => b.Position == new Vector(0, 4)),
            };

            var buildingsTable = new[,]
            {
                {buildingMocks[0], null, null, null, buildingMocks[4], null,},
                {buildingMocks[2], null, null, null, buildingMocks[1], null,},
                {buildingMocks[3], null, null, null, null,             null,},
                {null,             null, null, null, null,             null,},
                {null,             null, null, null, null,             null,},
                {null,             null, null, null, null,             null,},
            };

            var worldMock = new Mock<World>();
            worldMock
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns((Vector v) => buildingsTable[v.X, v.Y]);

            worldMock
                .SetupGet(w => w.Size)
                .Returns(() => new Vector(
                    buildingsTable.GetLength(0), 
                    buildingsTable.GetLength(1)));

            var player =
                Mock.Of<Player>(p => p.OwnBuildings == new List<Building> {buildingMocks[0], buildingMocks[1]});

            var manager = new VisionManager {VisionDefaultRange = 1};
            
            // act
            var result = manager.GetVision(worldMock.Object, player);
            
            // assert
            var expectedBuildingsTable = new[,]
            {
                {buildingMocks[0], null, null, null, buildingMocks[4], null,},
                {buildingMocks[2], null, null, null, buildingMocks[1], null,},
                {null,             null, null, null, null,             null,},
            };

            for (var x = 0; x < expectedBuildingsTable.GetLength(0); x++)
            for (var y = 0; y < expectedBuildingsTable.GetLength(1); y++)
            {
                Assert.AreEqual(expectedBuildingsTable[x, y], result.Buildings[x, y]);
            }
            
            Assert.AreEqual(new Vector(0, 0), result.Position);
        }
    }
}