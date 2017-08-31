using Isometric.Core.Managers;
using NUnit.Framework;

namespace Isometric.Core.Tests.Managers
{
    [TestFixture()]
    public class VisionManagerTests
    {
        [Test()]
        public void IsAreaOpened_ReturnsTrueWhenAreaIsMainPlayersOne()
        {
            // arrange
            var manager = new VisionManager();
            var area = new Area();
            var player = new Player {Area = area};

            // act
            var result = manager.IsAreaOpened(player, area);

            // assert
            Assert.AreEqual(true, result);
        }
    }
}