using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class SimpleAreaGeneratorTests
    {
        public void GenerateArea_ShouldGenerateAreaByChanceCollection()
        {
            //// arrange
            //var prototypes = new[]
            //{
            //    new Building("Plain"),
            //    new Building("Forest"),
            //    new Building("Lake"),
            //};

            //var template = new RandomGenerator(
            //    new ChanceCollection<Building>
            //    {
            //        new ChancePair<Building>(1, prototypes[0]),
            //        new ChancePair<Building>(1, prototypes[1]),
            //        new ChancePair<Building>(1, prototypes[2]),
            //    });

            //// act
            //var area = template.GenerateArea(new World { AreaWidth = 5 });

            //// assert
            //Assert.IsTrue(area.Buildings.Cast<Building>().All(b => prototypes.Contains(b.Prototype)));
        }

        [Test]
        public void GenerateArea_ShouldSetNeutralOwner()
        {
            //// arrange
            //var template = new RandomGenerator(
            //    new ChanceCollection<Building>
            //    {
            //        new ChancePair<Building>(1, new Building("Plain"))
            //    });

            //// act
            //var area = template.GenerateArea(new World { AreaWidth = 5 });

            //// assert
            //Assert.IsTrue(area.Buildings.Cast<Building>().All(b => b.Owner == Player.Neutral));
        }
    }
}