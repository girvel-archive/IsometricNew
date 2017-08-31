using System;
using Isometric.Core.Buildings;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.Buildings
{
    [TestFixture]
    public class PeriodIncomeBuildingTests
    {
        [Test]
        public void Tick_AddsResourcesEveryIncomePeriod()
        {
            // arrange
            var building = new PeriodIncomeBuilding
            {
                Prototype = new IncomeBuilding {IncomePeriod = TimeSpan.FromSeconds(1)},
                Finished = true,
                IncomePeriod = TimeSpan.FromSeconds(1),
                Incomes = new IResources[] { new DefaultResources(1) },
                Owner = Mock.Of<Player>(
                    p => p.Resources == (IResources) new DefaultResources()),
                Constants = new GameConstants {ZeroResources = new DefaultResources(),},
            };

            // act
            building.Tick(TimeSpan.FromSeconds(2.5));

            // assert
            Assert.AreEqual(2, building.Owner.Resources[0]);
        }
    }
}