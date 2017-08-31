using System;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Tick_FinishesResearchesWhenThereAreEnoughResearhPoints()
        {
            // arrange
            var player = new Player
            {
                CurrentResearchPoints = 1,
                CurrentResearch = Mock.Of<Research>(r => r.ResearchPointsRequired == 1)
            };

            // act
            player.Tick(new TimeSpan());

            // assert
            Assert.IsNull(player.CurrentResearch);
            Assert.AreEqual(1, player.ResearchedTechnologies.Count);
        }

        [Test]
        public void Tick_DecreasesCurrentResearchPointsWhenResearchFinishes()
        {
            // arrange
            var player = new Player
            {
                CurrentResearchPoints = 1,
                CurrentResearch = Mock.Of<Research>(r => r.ResearchPointsRequired == 1)
            };

            // act
            player.Tick(new TimeSpan());

            // assert
            Assert.AreEqual(0, player.CurrentResearchPoints);
        }

        [Test]
        public void Tick_InvokesCurrentResearchEndActionWhenItIsFinished()
        {
            // arrange
            var endActionWasInvoked = false;

            var researchMock = Mock.Of<Research>();
            researchMock.ResearchPointsRequired = 1;
            researchMock.EndAction = p => endActionWasInvoked = true;

            var player = new Player
            {
                CurrentResearchPoints = 1,
                CurrentResearch = researchMock,
            };

            // act
            player.Tick(new TimeSpan());

            // assert
            Assert.IsTrue(endActionWasInvoked);
        }
    }
}