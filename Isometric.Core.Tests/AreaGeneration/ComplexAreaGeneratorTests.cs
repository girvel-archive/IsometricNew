using System;
using Isometric.Core.AreaGeneration;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests.AreaGeneration
{
    [TestFixture]
    public class ComplexAreaGeneratorTests
    {
        [Test]
        public void GenerateArea_GeneratesAreaByGeneratorAndModifiesItByModifiers()
        {
            // arrange
            var resultArea = new Area();

            var innerGenerator = new Mock<IAreaGenerator>();
            innerGenerator
                .Setup(g => g.GenerateArea(It.IsAny<World>(), It.IsAny<Vector>()))
                .Callback(
                    (World w, Vector p) =>
                    {
                        w.SetArea(p, resultArea);
                    });

            var modificator = new Mock<IAreaModifier>();

            var complexGenerator = new ComplexAreaGenerator
            {
                Generator = innerGenerator.Object,
                Modifiers = new[] {modificator.Object, modificator.Object,},
            };

            var areas = new Area[,] {{null}};
            var world = Mock.Of<World>(w => w.Landscape == areas);
            var areaPosition = new Vector(0, 0);

            // act
            complexGenerator.GenerateArea(world, areaPosition);

            // assert
            Assert.AreEqual(resultArea, world.GetArea(areaPosition));
            throw new NotImplementedException();
            //modificator.Verify(m => m.Modify(world, position), Times.Exactly(2));
        }
    }
}