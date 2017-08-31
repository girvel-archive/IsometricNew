using Isometric.Core.Buildings;
using NUnit.Framework;

namespace Isometric.Core.Tests.Buildings
{
    [TestFixture]
    public class ScienceBuildingTests
    {
        [Test]
        public void Tick_AddsResearchPointsForPlayer()
        {
            // arrange
            var building = new ScienceBuilding
            {
                Workers = 1,
                ResearchIncome = 1,
                Prototype = new ScienceBuilding {Workers = 1},
                Owner = new Player {CurrentResearch = new Research()},
                Constants = new GameConstants(),
            };

            // act
            building.Tick(building.IncomePeriod);

            // assert
            Assert.AreEqual(1, building.Owner.CurrentResearchPoints);
        }

        [Test]
        public void Tick_AddedResearchPointsIncomeShouldDependFromDeltaTime()
        {
            // arrange
            var building = new ScienceBuilding
            {
                Workers = 1,
                ResearchIncome = 1,
                Prototype = new ScienceBuilding { Workers = 1 },
                Owner = new Player { CurrentResearch = new Research() },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(building.IncomePeriod + building.IncomePeriod);

            // assert
            Assert.AreEqual(2, building.Owner.CurrentResearchPoints);
        }

        [Test]
        public void Tick_AddedResearchPointsIncomeShouldDependFromWorkersNumber()
        {
            // arrange
            var building = new ScienceBuilding
            {
                Workers = 1,
                ResearchIncome = 2,
                Prototype = new ScienceBuilding { Workers = 2 },
                Owner = new Player { CurrentResearch = new Research() },
                Constants = new GameConstants(),
            };

            // act
            building.Tick(building.IncomePeriod);

            // assert
            Assert.AreEqual(1, building.Owner.CurrentResearchPoints);
        }

        [Test]
        public void Tick_DoNotAddResearchPointsWhenCurrentResearchIsNull()
        {
            // arrange
            var building = new ScienceBuilding
            {
                Workers = 1,
                ResearchIncome = 2,
                Prototype = new ScienceBuilding { Workers = 2 },
                Owner = new Player(),
                Constants = new GameConstants(),
            };

            // act
            building.Tick(building.IncomePeriod);

            // assert
            Assert.AreEqual(0, building.Owner.CurrentResearchPoints);
        }

        [Test]
        public void Tick_AddsResearchPointsSlowerWhenPeopleAreHungry()
        {
            // arrange
            var building = new ScienceBuilding
            {
                Workers = 1,
                ResearchIncome = 2,
                Prototype = new ScienceBuilding {Workers = 1},
                Owner = new Player {CurrentResearch = new Research()},
                Constants = new GameConstants {HungerK = 0.5f},
                ArePeopleHungry = true,
            };

            // act
            building.Tick(building.IncomePeriod);

            // assert
            Assert.AreEqual(1, building.Owner.CurrentResearchPoints);
        }
    }
}