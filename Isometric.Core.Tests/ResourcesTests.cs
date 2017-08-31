using Isometric.Core.Common;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class ResourcesTests
    {
        [Test]
        public void Plus_AddsArguments()
        {
            // arrange
            var r1 = new DefaultResources(12, 3);
            var r2 = new DefaultResources(4, 16);

            // act
            var result = r1.Added(r2);

            // assert
            Assert.AreEqual(result[0], 16);
            Assert.AreEqual(result[1], 19);
        }

        [Test]
        public void UnaryMinus_InvertsValues()
        {
            // arrange
            var r = new DefaultResources(8, -3);

            // act
            var result = r.Inverted();

            // assert
            Assert.AreEqual(result[0], -8);
            Assert.AreEqual(result[1], 3);
        }

        [Test]
        public void Substracted_SubstractsValues()
        {
            // arrange
            var r1 = new DefaultResources(12, 3);
            var r2 = new DefaultResources(4, 16);

            // act
            var result = r1.Substracted(r2);

            // assert
            Assert.AreEqual(result[0], 8);
            Assert.AreEqual(result[1], -13);
        }

        [Test]
        public void Multiply_MultipliesAllValues()
        {
            // arrange
            var r = new DefaultResources(12, -3);

            // act
            var result = r.Multiplied(3);

            // assert
            Assert.AreEqual(result[0], 36);
            Assert.AreEqual(result[1], -9);
        }

        [Test]
        public void Equals_ChecksUnequality()
        {
            // arrange
            var r1 = new DefaultResources(4, 3);
            var r2 = new DefaultResources(4, 16);

            // act
            var result = r1.Equals(r2);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ChecksEquality()
        {
            // arrange
            var r1 = new DefaultResources(4, 3);
            var r2 = new DefaultResources(4, 3);

            // act
            var result = r1.Equals<DefaultResources>(r2);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_ReturnsFalseWhenArgumentIsNotResource()
        {
            // arrange
            var r1 = new DefaultResources(4, 3);
            var r2 = new object();

            // act
            var result = r1.Equals(r2);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Enough_ReturnsTrueWhenYouHaveEnoughResources()
        {
            // arrange
            var r1 = new DefaultResources(4, 3);
            var r2 = new DefaultResources(3, 3);

            // act
            var result = r1.Enough(r2);

            // assert
            Assert.IsTrue(result);
        }
    }
}