using System;
using System.Diagnostics;
using System.Linq;
using Isometric.Core.Managers;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.Managers
{
    [TestFixture]
    public class ArmiesManagerTests
    {
        [Test]
        public void AddMovingTask_MovesArmy()
        {
            // arrange
            var manager = new ArmiesManager();
            
            var army = new Army
            {
                MovingTime = new TimeSpan(1),
                Prototype = new Army { MovingTime = new TimeSpan(1), },
                World = new World
                {
                    AreaWidth = 3,
                    Landscape = new[,]
                    {{
                        new Area
                        {
                            Buildings = new Building[3, 3],
                            [0, 0] = new Building(),
                            [0, 1] = new Building(),
                            [0, 2] = new Building(),
                            [1, 0] = new Building(),
                            [1, 1] = new Building(),
                            [1, 2] = new Building(),
                            [2, 0] = new Building(),
                            [2, 1] = new Building(),
                            [2, 2] = new Building(),
                        },
                    }},
                },
            };

            // act
            manager.AddMovingTask(army, new Vector(2, 2));
            manager.Tick(new TimeSpan(4));

            // assert
            Assert.AreEqual(army.World.GetBuilding(new Vector(2, 2)).Armies.First(), army);
        }

        [Test]
        public void AddMovingTask_MovesHungryArmySlower()
        {
            // arrange
            var manager = new ArmiesManager();

            var buildings = new[,]
            {
                {new Building(), new Building(), new Building(),},
                {new Building(), new Building(), new Building(),},
                {new Building(), new Building(), new Building(),},
            };
            
            var worldMock = new Mock<World>();
            worldMock
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns((Vector v) => buildings[v.X, v.Y]);
            
            worldMock
                .SetupGet(w => w.AreaWidth)
                .Returns(1);

            var army = new Army
            {
                MovingTime = new TimeSpan(1),
                Prototype = new Army
                {
                    MovingTime = new TimeSpan(1),
                },
                IsHungry = true,
                Constants = new GameConstants { HungerK = 0.5f, },
                World = worldMock.Object,
                Position = new Vector(0, 0),
            };

            // act
            manager.AddMovingTask(army, new Vector(2, 2));
            manager.Tick(new TimeSpan(4));

            // assert
            Assert.AreEqual(2, army.Position.X + army.Position.Y);
        }
    }
}