using System;
using Isometric.Core.Buildings;
using Isometric.Core.Time;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class BuildingTests
    {
        [Test]
        public void Tick_CallsTickDelegate()
        {
            // arrange
            var success = false;
            var b = new Building
            {
                TickAction = dt => success = true,
                Finished = true,
            };

            // act
            try
            {
                b.Tick(TimeSpan.Zero);
            }
            catch (NullReferenceException) { }

            // assert
            Assert.IsTrue(success);
        }

        [Test]
        public void Tick_DoesNotCallTemplateTickWhenItIsNull()
        {
            // arrange
            var building = new Building
            {
                TickAction = null,
                Finished = true,
                Constants = new GameConstants(),
            };

            // act
            building.Tick(new TimeSpan());

            // assert
            Assert.IsTrue(true); // when there is no exs
        }

        [Test]
        public void Tick_ChangesBuildingTimeIfBuildingIsNotFinished()
        {
            // arrange
            var building = new Building
            {
                TickAction = dt => { },
                BuildingTime = new TimeSpan(100),
                Builders = 1,
                Prototype = new Building { Builders = 1 },
                Constants = new GameConstants(),
                Owner = new Player { BirthrateK = 0 },
            };

            // act
            building.Tick(new TimeSpan(50));

            // assert
            Assert.IsTrue(building.BuildingTime.Ticks == 50);
        }

        [Test]
        public void Tick_BuildingTimeIncomeDependsFromBuilders()
        {
            // arrange
            var building = new Building
            {
                TickAction = dt => { },
                BuildingTime = new TimeSpan(100),
                Builders = 1,
                Prototype = new Building { Builders = 2 },
                Constants = new GameConstants(),
                Owner = new Player { BirthrateK = 0 },
            };

            // act
            building.Tick(new TimeSpan(100));

            // assert
            Assert.AreEqual(50, building.BuildingTime.Ticks);
        }

        [Test]
        public void Tick_DontChangeBuildingTimeIncomeWhenPrototypeBuildersEqualsZero()
        {
            // arrange
            var building = new Building
            {
                TickAction = dt => { },
                BuildingTime = new TimeSpan(100),
                Builders = 0,
                Prototype = new Building { Builders = 0 },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(new TimeSpan(100));

            // assert
            Assert.AreEqual(0, building.BuildingTime.Ticks);
        }

        [Test]
        public void Tick_FinishesBuildingWhenBuildingTimeEqualsZero()
        {
            // arrange
            var building = new Building
            {
                TickAction = dt => { },
                BuildingTime = new TimeSpan(100),
                Builders = 1,
                Prototype = new Building { Builders = 1 },
                Constants = new GameConstants(),
                Owner = new Player { BirthrateK = 0 },
            };

            // act
            ((ITimeObject)building).Tick(new TimeSpan(100));

            // assert
            Assert.IsTrue(building.Finished);
        }

        [Test]
        public void CreateByPrototype_ClonesPrototype()
        {
            // arrange
            var prototype = new Building();

            // act
            var building = Building.CreateByPrototype(prototype, new Player(), null, new Vector(), null);

            // assert
            foreach (var field in building.GetType().GetFields())
            {
                Assert.AreEqual(field.GetValue(prototype), field.GetValue(building));
            }
        }

        [Test]
        public void CreateByPrototype_ChangesPrototypeOwnerAreaAndPosition()
        {
            // arrange
            var prototype = new Building();
            var player = new Player();
            var world = new World();
            var position = new Vector();

            // act
            var building = Building.CreateByPrototype(prototype, player, world, position, null);

            // assert
            Assert.AreEqual(player, building.Owner);
            Assert.AreEqual(prototype, building.Prototype);
            Assert.AreEqual(world, building.World);
            Assert.AreEqual(position, building.Position);
        }

        [Test]
        public void CreateByPrototype_SetsDefaultValues()
        {
            // arrange
            var prototype = new Building() { Builders = 5, };
            var player = new Player();

            // act
            var building = Building.CreateByPrototype(prototype, player, null, new Vector(), null);

            // assert
            Assert.AreEqual(building.Builders, 0);
        }

        [Test]
        public void CreateByPrototype_SetsDefaultValuesForWorkerBuilding()
        {
            // arrange
            var prototype = new WorkerBuilding { Workers = 1 };
            var player = new Player();

            // act
            var building = (WorkerBuilding)Building.CreateByPrototype(prototype, player, null, new Vector(), null);

            // assert
            Assert.AreEqual(building.Workers, 0);
        }

        [Test]
        public void CreateByPrototype_GetsDefaultValuesFromPreviousBuilding()
        {
            // arrange
            var from = new Building { FreePeople = 1, Builders = 2 };
            var prototype = new Building();

            // act
            var to = Building.CreateByPrototype(prototype, new Player(), null, new Vector(), null, from);

            // assert
            Assert.AreEqual(1, to.TotalPeople);
            Assert.AreEqual(2, to.Builders);
        }

        [Test]
        public void Tick_CreatesOnePersonPeriodically()
        {
            // arrange
            var building = new Building
            {
                Finished = true,
                Builders = 2,
                FreePeople = 8,
                Constants = new GameConstants { PeopleGenerationSize = TimeSpan.FromSeconds(10) },
                Owner = new Player(),
            };

            // act
            building.Tick(TimeSpan.FromSeconds(1));

            // assert
            Assert.AreEqual(9, building.TotalPeople);
        }

        [Test]
        public void Tick_CreationPeriodDependsFromPlayerBirthrateCoefficient()
        {
            // arrange
            var building = new Building
            {
                Finished = true,
                Builders = 2,
                FreePeople = 8,
                Constants = new GameConstants
                {
                    PeopleGenerationSize = TimeSpan.FromSeconds(10),
                    PersonConsumption = new DefaultResources(0),
                    PersonConsumptionPeriod = TimeSpan.FromMinutes(0),
                },
                Owner = new Player { BirthrateK = 2, },
            };

            // act
            building.Tick(TimeSpan.FromSeconds(1));

            // assert
            Assert.AreEqual(10, building.TotalPeople);
        }

        [Test]
        public void Tick_InvokesOnPeopleCreated()
        {
            // arrange
            var onPeopleCreatedWasCalled = false;
            var building = new Building
            {
                Finished = true,
                Builders = 2,
                FreePeople = 8,
                Constants = new GameConstants
                {
                    PeopleGenerationSize = TimeSpan.FromSeconds(10),
                },
                OnPeopleCreated = n => onPeopleCreatedWasCalled = true,
                Owner = new Player { BirthrateK = 1 },
            };

            // act
            building.Tick(TimeSpan.FromSeconds(1));

            // assert
            Assert.IsTrue(onPeopleCreatedWasCalled);
        }

        [Test]
        public void Tick_DecrementsOwnersResourcesByPersonConsumption()
        {
            // arrange
            var building = new Building
            {
                Finished = true,
                Builders = 2,
                FreePeople = 8,
                Constants = new GameConstants
                {
                    PeopleGenerationSize = TimeSpan.FromSeconds(10),
                    PersonConsumption = new DefaultResources(1),
                    PersonConsumptionPeriod = TimeSpan.FromMinutes(1),
                },
                Owner = new Player { BirthrateK = 0, Resources = new DefaultResources(3, 0), },
            };

            // act
            building.Tick(TimeSpan.FromMinutes(2));

            // assert
            Assert.AreEqual(1, building.Owner.Resources[0]);
        }

        [Test]
        public void Tick_InvokesArmiesTick()
        {
            // arrange
            var armyMock = new Mock<Army>();
            var building = new Building
            {
                Finished = true,
                Constants = new GameConstants(),
                Owner = new Player { BirthrateK = 0, Resources = new DefaultResources(0), },
                Armies = { armyMock.Object, armyMock.Object },
            };

            // act
            building.Tick(TimeSpan.Zero);

            // assert
            armyMock.Verify(a => a.Tick(TimeSpan.Zero), Times.Exactly(2));
        }

        [Test]
        public void Tick_MakesPeopleHungryWhenOwnerHasNotEnoughResources()
        {
            // arrange
            var building = new Building
            {
                Finished = true,
                Builders = 2,
                FreePeople = 8,
                Constants = new GameConstants
                {
                    PeopleGenerationSize = TimeSpan.FromSeconds(10),
                    PersonConsumption = new DefaultResources(4),
                    PersonConsumptionPeriod = TimeSpan.FromMinutes(1),
                },
                Owner = new Player { BirthrateK = 0, Resources = new DefaultResources(3), },
            };

            // act
            building.Tick(TimeSpan.FromMinutes(1));

            // assert
            Assert.IsTrue(building.ArePeopleHungry);
        }

        [Test]
        public void Tick_BuildsSlowerIfPeopleAreHungry()
        {
            // arrange
            var building = new Building
            {
                TickAction = dt => { },
                BuildingTime = new TimeSpan(100),
                Builders = 1,
                Prototype = new Building { Builders = 1 },
                Constants = new GameConstants { HungerK = 0.5f, },
                Owner = new Player { BirthrateK = 0, },
                ArePeopleHungry = true,
            };

            // act
            building.Tick(new TimeSpan(50));

            // assert
            Assert.AreEqual(75, building.BuildingTime.Ticks);
        }

        [Test]
        public void ContainsPeople_Set_RecalculatesOwnersMaxPeople()
        {
            // arrange
            var building = new Building
            {
                Owner = new Player(),
            };

            // act
            building.ContainsPeople = 1;

            // assert
            Assert.AreEqual(1, building.Owner.MaxPeople);
        }

        [Test]
        public void Owner_Set_RecalculatesOwnersMaxPeople()
        {
            // arrange
            var oldOwner = new Player();
            var building = new Building
            {
                Owner = oldOwner,
                ContainsPeople = 1,
            };

            // act
            building.Owner = new Player();

            // assert
            Assert.AreEqual(1, building.Owner.MaxPeople);
            Assert.AreEqual(0, oldOwner.MaxPeople);
        }

        [Test]
        public void Tick_PeopleDoNotIncreaseWhenThereIsNotEnoughHouses()
        {
            // arrange
            var building = new Building
            {
                Finished = true,
                FreePeople = 10,
                Constants = new GameConstants { PeopleGenerationSize = TimeSpan.FromSeconds(10) },
                Owner = new Player(),
                ContainsPeople = 15,
            };

            // act
            building.Tick(TimeSpan.FromSeconds(10));

            // assert
            Assert.AreEqual(15, building.TotalPeople);
        }

        [Test]
        public void FreePeople_Set_ChangesOwnersTotalPeople()
        {
            // arrange
            var building = new Building
            {
                Owner = new Player {TotalPeople = 4},
            };

            // act
            building.FreePeople = 2;

            // assert
            Assert.AreEqual(6, building.Owner.TotalPeople);
        }
    }
}