using System;
using System.Linq;
using Isometric.Core.Vectors;
using Moq;
using NUnit.Framework;

namespace Isometric.Core.Tests
{
    [TestFixture]
    public class ArmyTests
    {
        [Test]
        public void Tick_DecrementsOwnerResourcesByConsumption()
        {
            // arrange
            var army = new Army
            {
                Owner = new Player { Resources = new DefaultResources(3) },
                Consumption = new DefaultResources(1),
                ConsumptionPeriod = TimeSpan.FromDays(1),
            };

            // act
            army.Tick(TimeSpan.FromDays(2));

            // assert
            Assert.AreEqual(1, army.Owner.Resources[0]);
        }

        [Test]
        public void Tick_MakesArmyHungryWhenOwnerHasNotEnoughResources()
        {
            // arrange
            var army = new Army
            {
                Owner = new Player { Resources = new DefaultResources(3) },
                Consumption = new DefaultResources(4),
                ConsumptionPeriod = TimeSpan.FromDays(1),
                Constants = new GameConstants { ZeroResources = new DefaultResources() },
            };

            // act
            army.Tick(TimeSpan.FromDays(1));

            // assert
            Assert.IsTrue(army.IsHungry);
        }

        [Test]
        public void Attack_DecreasesCombatPowersByDamage()
        {
            // arrange
            var army1 = new Army { LifePoints = 10, Damage = 1, };
            var army2 = new Army { LifePoints = 5, Damage = 2, };

            // act
            army1.Attack(army2);

            // assert
            Assert.AreEqual(8, army1.LifePoints);
            Assert.AreEqual(4, army2.LifePoints);
        }

        [Test]
        public void SetPosition_RemovesArmyFromOldBuildingListAndAddsToNewOne()
        {
            // arrange
            var area = Mock.Of<Area>();
            area.Buildings = new[,] { { new Building(), new Building(), } };
            
            var world = new Mock<World>();
            world
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns<Vector>(v => area[v]);

            var army = new Army
            {
                World = world.Object,
                Position = new Vector(0, 0),
            };

            // act           
            army.Position = new Vector(0, 1);

            // assert
            Assert.IsFalse(area[new Vector(0, 0)].Armies.Any());
            Assert.AreEqual(army, area[new Vector(0, 1)].Armies[0]);
        }

        [Test]
        public void SetPosition_AttackIfThereIsEnemyAtNewPosition()
        {
            // arrange
            var enemy = new Army
            {
                Owner = new Player(),
                LifePoints = 10,
                Damage = 1,
            };

            var area = Mock.Of<Area>();
            area.Buildings = new[,]
            {
                {
                    new Building(),
                    new Building {Armies = {enemy,},},
                },
            };
            
            var world = new Mock<World>();
            world
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns<Vector>(v => area[v]);

            var army = new Army
            {
                World = world.Object,
                Position = new Vector(0, 0),
                LifePoints = 5,
                Damage = 2,
            };

            // act
            army.Position = new Vector(0, 1);

            // assert
            Assert.AreEqual(8, enemy.LifePoints);
            Assert.AreEqual(4, army.LifePoints);
        }

        [Test]
        public void SetPosition_MovesIfEnemyDiesBecauseOfAttack()
        {
            // arrange
            var area = Mock.Of<Area>();

            var world = new Mock<World>();
            world
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns<Vector>(v => area[v]);
            
            var enemy = new Army
            {
                World = world.Object,
                Owner = new Player(),
                LifePoints = 2,
                Damage = 1,
                Constants = new GameConstants{ZeroResources = new DefaultResources()},
            };

            area.Buildings = new[,]
            {
                {
                    new Building(),
                    new Building {Armies = {enemy,},},
                },
            };

            var army = new Army
            {
                World = world.Object,
                Position = new Vector(0, 0),
                LifePoints = 5,
                Damage = 2,
                Owner = new Player{Resources = new DefaultResources()},
            };

            // act
            army.Position = new Vector(0, 1);

            // assert
            Assert.IsFalse(area[new Vector(0, 0)].Armies.Any());
            Assert.IsTrue(area[new Vector(0, 1)].Armies.Contains(army));
        }

        [Test]
        public void GetDamageFor_IncreasesDamageIfThereIsBonusForEnemysArmor()
        {
            // arrange
            var army = new Army
            {
                Damage = 10,
                BonusDamageArmorType = ArmorType.Heavy,
                BonusDamage = 5,
            };

            var enemy = Mock.Of<IMilitaryObject>(m => m.Armor == ArmorType.Heavy);

            // act
            var result = army.GetDamageFor(enemy);

            // assert
            Assert.AreEqual(15, result);
        }

        [Test]
        public void Destroy_RemovesArmy()
        {
            // arrange
            var area = Mock.Of<Area>();
            area.Buildings = new[,] { { new Building(), } };

            var world = new Mock<World>();
            world
                .Setup(w => w.GetBuilding(It.IsAny<Vector>()))
                .Returns<Vector>(v => area[v]);

            var army = new Army
            {
                World = world.Object,
                Position = new Vector(0, 0),
            };

            // act           
            army.Destroy(null);

            // assert
            Assert.IsFalse(area[army.Position].Armies.Any());
        }
    }
}