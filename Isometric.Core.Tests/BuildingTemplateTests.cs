using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class BuildingTemplateTests
    {
        [Test] public void IsUpgradePossible_ReturnsTrueWhenBuildingCanBeUpgraded()
        {
            // arrangev
            var player = new Player
            {
                Resources = Mock.Of<IResources>(r => r.Enough(It.IsAny<IResources>()) == true),
                ResearchedTechnologies = new List<Research> { new Research() },
            };
            var to = new Building
            {
                RequiredResearches = player.ResearchedTechnologies.ToArray(),
            };

            var from = new Building
            {
                Upgrades = new[] {to},
                Owner = player,
            };

            // act
            var result = from.IsUpgradePossible(to);

            // assert
            Assert.IsTrue(result);
        }
    }
}