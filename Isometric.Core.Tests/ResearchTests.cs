using System.Linq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class ResearchTests
    {
        [Test]
        public void GetAllChildren_ContainsAllChildrenWithoutRepeats()
        {
            // arrange
            var researches = new[]
            {
                new Research("1", 1, new Research[0]),
                new Research("2", 1, new Research[0]),
                new Research("3", 1, new Research[0]),
            };

            var root = new Research(
                "A",
                1,
                new[] { researches[0], researches[1] });

            researches[0].Children = new[]
            {
                researches[1],
                researches[2],
            };

            // act
            var allResearches = root.GetAllChildren();

            // assert
            Assert.AreEqual(researches.Length, allResearches.Count());
            Assert.IsTrue(allResearches.All(r => researches.Contains(r)));
        }
    }
}