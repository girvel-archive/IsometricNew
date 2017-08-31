using System;
using Isometric.Core.Buildings;
using NUnit.Framework;

namespace Isometric.Core.Tests.Buildings
{
    [TestFixture]
    public class IncomeBuildingTests
    {
        [Test]
        public void Tick_AddsFirstPossibleIncomeToCurrentResources()
        {
            // arrange
            var building = new IncomeBuilding
            {
                Incomes = new IResources[]
                {
                    new DefaultResources(100, -100),
                    new DefaultResources(100, 100, -100), 
                },
                Workers = 1,
                Finished = true,
                Owner = new Player {Resources = new DefaultResources(0, 0, 50)},
                Prototype = new IncomeBuilding {Workers = 1},
                Constants = new GameConstants
                {
                    PersonConsumption = new DefaultResources(0),
                    PersonConsumptionPeriod = TimeSpan.FromMinutes(0),
                },
            };

            // act
            building.Tick(new TimeSpan(0, 1, 0));

            // assert
            Assert.AreEqual(50, building.Owner.Resources[0]);
        }

        [Test]
        public void Tick_DoesNotAddIncomeIfItIsNotFinished()
        {
            // arrange
            var b = new IncomeBuilding
            {
                Incomes = new IResources[] { new DefaultResources(100) },

                Workers = 1,
                BuildingTime = new TimeSpan(1),
                Owner = new Player { Resources = new DefaultResources() },
                Builders = 1,
                Prototype = new IncomeBuilding { Builders = 1},
                Constants = new GameConstants(),
            };

            // act
            b.Tick(new TimeSpan(0));

            // assert
            Assert.AreEqual(0, b.Owner.Resources[0]);
        }

        [Test]
        public void Tick_AddedIncomeShouldDependFromDeltaTime()
        {
            // arrange
            var building = new IncomeBuilding
            {
                Incomes = new IResources[] { new DefaultResources(100) },
                Workers = 1,
                Finished = true,
                Owner = new Player { Resources = new DefaultResources() },
                Prototype = new IncomeBuilding { Workers = 1 },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(new TimeSpan(0, 2, 0));

            // assert
            Assert.AreEqual(200, building.Owner.Resources[0]);
        }

        [Test]
        public void Tick_AddedIncomeShouldDependFromWorkers()
        {
            // arrange
            var building = new IncomeBuilding
            {
                Incomes = new IResources[] { new DefaultResources(100) },
                Workers = 1,
                Finished = true,
                Owner = new Player { Resources = new DefaultResources() },
                Prototype = new IncomeBuilding { Workers = 2 },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(new TimeSpan(0, 1, 0));

            // assert
            Assert.AreEqual(50, building.Owner.Resources[0]);
        }

        [Test]
        public void Tick_IncomeDependsFromAcceleration()
        {
            // arrange
            var building = new IncomeBuilding
            {
                World = new World(),
                Incomes = new IResources[] { new DefaultResources(100) },
                Workers = 1,
                Finished = true,
                Owner = new Player { Resources = new DefaultResources(0, 50) },
                Prototype = new IncomeBuilding { Workers = 1 },
                Acceleration = new IncomeBuilding.AccelerationCollection
                {
                    [new DefaultResources(0, 50)] = 2, 
                },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(building.IncomePeriod);

            // assert
            Assert.AreEqual(200, building.Owner.Resources[0]);
            Assert.AreEqual(0, building.Owner.Resources[1]);
        }

        [Test]
        public void Tick_IncomeIsAcceleratingByTheMostEffectiveWay()
        {
            // arrange
            var building = new IncomeBuilding
            {
                World = new World(),
                Incomes = new IResources[] { new DefaultResources(100) },
                Workers = 1,
                Finished = true,
                Owner = new Player { Resources = new DefaultResources(0, 50, 25) },
                Prototype = new IncomeBuilding { Workers = 1 },
                Acceleration = new IncomeBuilding.AccelerationCollection
                {
                    [new DefaultResources(0, 50)] = 2,
                    [new DefaultResources(0, 0, 50)] = 3,
                },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(new TimeSpan(0, 1, 0));

            // assert
            Assert.AreEqual(200, building.Owner.Resources[0]);
            Assert.AreEqual(0, building.Owner.Resources[1]);
        }

        [Test]
        public void Tick_AddsLessResourcesWhenPeopleAreHungry()
        {
            // arrange
            var building = new IncomeBuilding
            {
                Incomes = new IResources[] { new DefaultResources(100) },
                Workers = 1,
                Finished = true,
                Owner = new Player { Resources = new DefaultResources() },
                Prototype = new IncomeBuilding { Workers = 1 },
                Constants = new GameConstants
                {
                    HungerK = 0.5f,
                },
                ArePeopleHungry = true,
            };

            // act
            building.Tick(new TimeSpan(0, 1, 0));

            // assert
            Assert.AreEqual(50, building.Owner.Resources[0]);
        }
    }
}