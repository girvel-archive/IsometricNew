using System;
using Isometric.Core.Buildings;
using Isometric.Core.Managers;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.Managers
{
    [TestFixture]
    public class PeopleManagerTests
    {
        [Test]
        public void Tick_CallsAllPeopleManagementActions()
        {
            // arrange
            var counter = 0;
            var manager = new PeopleManager();
            manager.PeopleManagementActions.Add(() => counter++ == 0);
            manager.PeopleManagementActions.Add(() => counter++ == 0);

            // act
            manager.Tick(new TimeSpan());

            // assert
            Assert.AreEqual(2, counter);
        }

        [Test]
        public void Tick_RemovesPeopleManagementActionsWhenTheyReturnsTrue()
        {
            // arrange
            Func<bool> trueAction, falseAction;
            var manager = new PeopleManager();
            manager.PeopleManagementActions.Add(trueAction = () => true);
            manager.PeopleManagementActions.Add(falseAction = () => false);

            // act
            manager.Tick(new TimeSpan());

            // assert
            Assert.IsTrue(manager.PeopleManagementActions.Contains(falseAction));
            Assert.IsFalse(manager.PeopleManagementActions.Contains(trueAction));
        }

        [Test]
        public void TryAddWorkers_AddsWorkersIfThereIsEnoughFreePeople()
        {
            // arrange
            var buildings = new[,]
            {
                {
                    Mock.Of<IncomeBuilding>(
                        b =>
                            b.Finished == true && b.Workers == 0 && ((Building) b).TotalPeople == 10 &&
                            b.Prototype == Mock.Of<IncomeBuilding>(p => p.Workers == 1))
                }
            };
            buildings[0, 0].Owner = Mock.Of<Player>(p => p.Area == Mock.Of<Area>(a => a.Buildings == buildings));

            var manager = new PeopleManager();

            // act
            manager.TryAddWorkers(buildings[0, 0], 1);

            // assert
            Assert.AreEqual(1, buildings[0, 0].Workers);
        }

        [Test]
        public void TryAddWorkers_RemovesWorkersIfThereIsEnoughWorkers()
        {
            // arrange
            var buildings = new[,]
            {
                {
                    Mock.Of<IncomeBuilding>(
                        b =>
                            b.Finished == true && b.Workers == 1 && ((Building) b).TotalPeople == 9 &&
                            b.Prototype == Mock.Of<IncomeBuilding>(p => p.Workers == 1))
                }
            };
            buildings[0, 0].Owner = Mock.Of<Player>(p => p.Area == Mock.Of<Area>(a => a.Buildings == buildings));

            var manager = new PeopleManager();

            // act
            manager.TryAddWorkers(buildings[0, 0], -1);

            // assert
            Assert.AreEqual(0, buildings[0, 0].Workers);
        }

        [Test]
        public void TryAddBuilders_AddsBuildersIfThereIsEnoughFreePeople()
        {
            // arrange
            var buildings = new[,]
            {
                {
                    Mock.Of<Building>(
                        b =>
                            b.Finished == false &&
                            b.Builders == 0 && b.TotalPeople == 10 &&
                            b.Prototype == Mock.Of<IncomeBuilding>(
                                p => p.Workers == 0 && p.Builders == 1))
                }
            };
            buildings[0, 0].Owner = Mock.Of<Player>(p => p.Area == Mock.Of<Area>(a => a.Buildings == buildings));

            var manager = new PeopleManager();

            // act
            manager.TryAddBuilders(buildings[0, 0], 1);

            // assert
            Assert.AreEqual(1, buildings[0, 0].Builders);
        }

        [Test]
        public void TryAddBuilders_RemovesBuildersIfThereIsEnoughBuilders()
        {
            // arrange
            var buildings = new[,]
            {
                {
                    Mock.Of<Building>(
                        b =>
                            b.Finished == false &&
                            b.Builders == 1 && b.TotalPeople == 9 &&
                            b.Prototype == Mock.Of<IncomeBuilding>(
                                p => p.Workers == 0 && p.Builders == 1))
                }
            };
            buildings[0, 0].Owner = Mock.Of<Player>(p => p.Area == Mock.Of<Area>(a => a.Buildings == buildings));

            var manager = new PeopleManager();

            // act
            manager.TryAddBuilders(buildings[0, 0], -1);

            // assert
            Assert.AreEqual(0, buildings[0, 0].Builders);
        }
    }
}