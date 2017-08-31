using System;
using System.Linq;
using Isometric.Core.Buildings;
using Isometric.Core.Vectors;
using NUnit.Framework;

namespace Isometric.Core.Tests.Buildings
{
    [TestFixture]
    public class ArmyBuildingTests
    {
        [Test]
        public void Tick_DecreasesArmyCreationPeriod()
        {
            // arrange
            var b = new ArmyBuilding
            {
                Finished = true,
                ArmyCreationTime = TimeSpan.FromSeconds(5),
                Prototype = new ArmyBuilding {ArmyCreationTime = TimeSpan.FromSeconds(5)},
                Owner = new Player { Resources = new DefaultResources(100) },
                Constants = new GameConstants(),
                ArmyPrototype = new Army { Price = new DefaultResources(),}
            };

            // act
            b.Tick(TimeSpan.FromSeconds(3));

            // assert
            Assert.AreEqual(2, b.ArmyCreationTime.TotalSeconds);
        }

        [Test]
        public void Tick_CreatesArmyEveryArmyCreationPeriodAndDecreasesFreePeople()
        {
            // arrange
            var b = new ArmyBuilding
            {
                Finished = true,
                ArmyCreationTime = TimeSpan.FromSeconds(5),
                ArmyPrototype = new Army {RequiredPeople = 1, Price = new DefaultResources(), },
                Prototype = new ArmyBuilding {ArmyCreationTime = TimeSpan.FromSeconds(5)},
                FreePeople = 2,
                Owner = new Player { Resources = new DefaultResources(100) },
                Constants = new GameConstants(),
            };

            b.World = new World
            {
                Landscape = new[,] {{new Area {Buildings = new Building[,] {{b}}}}},
            };

            b.World.GetArea(new Vector(0, 0)).World = b.World;

            // act
            b.Tick(TimeSpan.FromSeconds(10));

            // assert
            Assert.IsTrue(b.Armies.All(a => a.Prototype == b.ArmyPrototype));
            Assert.AreEqual(2, b.Armies.Count);
            Assert.AreEqual(0, b.TotalPeople);
        }

        [Test]
        public void Tick_CreatesArmyOnlyIfPlayerHasEnoughResources()
        {
            // arrange
            var b = new ArmyBuilding
            {
                Finished = true,
                ArmyCreationTime = TimeSpan.FromSeconds(5),
                ArmyPrototype = new Army { RequiredPeople = 1, Price = new DefaultResources(50) },
                Prototype = new ArmyBuilding { ArmyCreationTime = TimeSpan.FromSeconds(10) },
                FreePeople = 2,
                Owner = new Player { Resources = new DefaultResources(100)},
                Constants = new GameConstants(),
            };

            b.World = new World
            {
                Landscape = new[,] {{new Area {Buildings = new Building[,] {{b}}}}},
            };

            b.World.GetArea(new Vector(0, 0)).World = b.World;

            // act
            b.Tick(TimeSpan.FromSeconds(10));

            // assert
            Assert.AreEqual(50, b.Owner.Resources[0]);
        }

        [Test]
        public void Tick_TrainsArmySlowerWhenPeopleAreHungry()
        {
            // arrange
            var b = new ArmyBuilding
            {
                Finished = true,
                ArmyCreationTime = TimeSpan.FromSeconds(5),
                ArmyPrototype = new Army { RequiredPeople = 1, Price = new DefaultResources(), },
                Prototype = new ArmyBuilding { ArmyCreationTime = TimeSpan.FromSeconds(5) },
                FreePeople = 2,
                Owner = new Player { Resources = new DefaultResources(100) },
                Constants = new GameConstants {HungerK = 0.5f},
                ArePeopleHungry = true,
            };

            b.World = new World
            {
                Landscape = new[,] {{new Area {Buildings = new Building[,] {{b}}}}},
            };

            b.World.GetArea(new Vector(0, 0)).World = b.World;

            // act
            b.Tick(TimeSpan.FromSeconds(10));

            // assert
            Assert.AreEqual(1, b.Armies.Count);
        }
    }
}