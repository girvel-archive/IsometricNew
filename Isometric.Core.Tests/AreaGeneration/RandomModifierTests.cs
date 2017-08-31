using NUnit.Framework;

namespace Isometric.Core.Tests.AreaGeneration
{
    [TestFixture()]
    public class RandomModifierTests
    {
        [Test]
        public void Modify_UsesReplacersToModifyArea()
        {
            //// arrange
            //var buildings = new[] {new Building(), new Building(),};

            //var modifier = new RandomModifier
            //{
            //    Replacers = new ChanceCollection<Replacer>
            //    {
            //        new ChancePair<Replacer>(1, b => buildings[0]),
            //        new ChancePair<Replacer>(1, b => buildings[1]),
            //    },
            //};

            //var position = new Vector(0, 0);

            //var world = Mock.Of<World>();
            //world.Landscape = new[,] {{Mock.Of<Area>()}};
            //world[position].Buildings = new[,]
            //{
            //    {
            //        new Building(),
            //        new Building(),
            //    },
            //    {
            //        new Building(),
            //        new Building(),
            //    },
            //};

            //// act
            //modifier.Modify(world, position);

            //// assert
            //Assert.IsTrue(world[position].Buildings.OfType<Building>().Any(b => buildings.Contains(b)));
        }
    }
}