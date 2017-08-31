using System;
using System.Linq;
using Isometric.Core.AreaGeneration;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.AreaGeneration
{
    [TestFixture()]
    public class RandomPositionModifierTests
    {
        [Test]
        public void Modify_CreatesFixedNumberOfBuildingsAtRandomPositions()
        {
            // arrange
            var modifier = new RandomPositionModifier
            {
                Number = 2,
                Prototype = new Building(),
            };

            var areaPosition = new Vector(0, 0);

            var world = Mock.Of<World>();
            world.Landscape = new[,] {{Mock.Of<Area>()}};
            world.GetArea(areaPosition).Buildings = new[,]
            {
                {
                    new Building(),
                    new Building(),
                },
                {
                    new Building(),
                    new Building(),
                },
            };

            // act
            throw new NotImplementedException();
            //modifier.Modify(world, position);

            // assert
            Assert.AreEqual(2, world.GetArea(areaPosition).Buildings.OfType<Building>().Count(b => b.Prototype == modifier.Prototype));
        }

        [Test]
        public void Modify_ThrowsInvalidOperationExceptionIfItOverflowsArea()
        {
            // arrange
            var modifier = new RandomPositionModifier
            {
                Number = 5,
                Prototype = new Building(),
            };

            var areaPosition = new Vector(0, 0);

            var world = Mock.Of<World>();
            world.Landscape = new[,] { { Mock.Of<Area>() } };
            world.GetArea(areaPosition).Buildings = new[,]
            {
                {
                    new Building(),
                    new Building(),
                },
                {
                    new Building(),
                    new Building(),
                },
            };

            // act
            var wasInvalidOperationExceptionThrown = false;
            try
            {
                throw new NotImplementedException();
                //modifier.Modify(world, position);
            }
            catch (InvalidOperationException)
            {
                wasInvalidOperationExceptionThrown = true;
            }

            // assert
            Assert.IsTrue(wasInvalidOperationExceptionThrown);
        }
    }
}