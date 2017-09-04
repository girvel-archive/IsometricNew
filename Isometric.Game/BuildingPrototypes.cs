using System;
using System.Linq;
using System.Reflection;
using Isometric.Core;
using Isometric.Core.Buildings;

namespace Isometric.Game
{
    internal class BuildingPrototypes
    {
        private static BuildingPrototypes _current;
        public static BuildingPrototypes Current = _current ?? (_current = new BuildingPrototypes());

        public Building[] GetAllPrototypes() =>
            typeof (BuildingPrototypes).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.FieldType == typeof (Building))
                .Select(f => (Building) f.GetValue(this))
                .ToArray();

        public static IncomeBuilding.AccelerationCollection Acceleration =
            new IncomeBuilding.AccelerationCollection
            {
                [new Resources {Instruments = 1}] = 1.5f,
            };

        public Building
            Plain = new Building(
                "Plain",
                new Resources())
            {
                Finished = true,
            },

            Forest = new Building(
                "Forest",
                new Resources())
            {
                Finished = true,
            },

            UndergroundHouse = new IncomeBuilding(
                "Underground house",
                new Resources {Wood = 50,},
                1,
                new IResources[] {new Resources {Wood = -2, RawFood = -5, Food = 5,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromSeconds(30),
                LifePoints = 100,
                Acceleration = Acceleration,
                ContainsPeople = 5,
            },

            WoodenHouse = new IncomeBuilding(
                "Wooden house",
                new Resources {Wood = 100,},
                1,
                new IResources[] {new Resources {Wood = -2, RawFood = -5, Food = 5,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromSeconds(90),
                LifePoints = 150,
                Acceleration = Acceleration,
                RequiredResearches = new[] { Researches.Current.WoodenArchitecture },
                ContainsPeople = 10,
            },

            Sawmill = new IncomeBuilding(
                "Sawmill",
                new Resources {Wood = 80,},
                5,
                new IResources[] {new Resources {Wood = 5,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromSeconds(60),
                LifePoints = 80,
                Acceleration = Acceleration,
            },

            HuntersShack = new IncomeBuilding(
                "Hunter's shack",
                new Resources {Wood = 80,},
                5,
                new IResources[] {new Resources {RawFood = 5,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromSeconds(60),
                LifePoints = 80,
                Acceleration = Acceleration,
            },

            SpiritHouse = new ScienceBuilding(
                "Spirit house",
                new Resources {Wood = 200,},
                1,
                2)
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromSeconds(60),
                LifePoints = 80,
            },

            Barracks = new ArmyBuilding(
                "Barracks",
                new Resources {Wood = 200},
                new Army(
                    "Infantry",
                    10,
                    TimeSpan.FromSeconds(3),
                    10,
                    Resources.Zero,
                    new Resources {Food = 10})
                {
                    Damage = 10,
                    Armor = ArmorType.Medium,
                },
                TimeSpan.FromMinutes(5))
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromSeconds(20),
            },

            HeavyBarracks = new ArmyBuilding(
                "Barracks of heavy infantry",
                new Resources {Wood = 500},
                new Army(
                    "Heavy infantry",
                    15,
                    TimeSpan.FromSeconds(6),
                    10,
                    Resources.Zero,
                    new Resources {Food = 10})
                {
                    Armor = ArmorType.Heavy,
                    BonusDamageArmorType = ArmorType.Building,
                    BonusDamage = 10,
                },
                TimeSpan.FromHours(1))
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromHours(1),
                RequiredResearches = new[] { Researches.Current.HeavyInfantry },
            },

            CoalMine = new IncomeBuilding(
                "Coal mine",
                new Resources {Wood = 1000},
                10,
                new IResources[] {new Resources {Coal = 10,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromHours(6),
                LifePoints = 500,
                Acceleration = Acceleration,
                RequiredResearches = new[] { Researches.Current.CoalMines },
            },

            Field = new PeriodIncomeBuilding(
                "Field",
                Resources.Zero,
                5,
                new IResources[] {new Resources {Corn = 10,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromMinutes(5),
                LifePoints = 10,
                Acceleration = Acceleration,
                RequiredResearches = new[] {Researches.Current.Agriculture},
            },

            Bakery = new IncomeBuilding(
                "Bakery",
                new Resources {Wood = 100},
                2,
                new IResources[] {new Resources {Wood = -1, RawFood = -5, Food = 5,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromMinutes(30),
                LifePoints = 100,
                Acceleration = Acceleration,
                RequiredResearches = new[] {Researches.Current.Bakeries},
            },

            Mill = new IncomeBuilding(
                "Mill",
                new Resources {Wood = 500},
                10,
                new IResources[] {new Resources {Corn = -15, RawFood = 15,}})
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromHours(6),
                LifePoints = 500,
                Acceleration = Acceleration,
                RequiredResearches = new[] { Researches.Current.Agriculture },
            },

            Workshop = new IncomeBuilding(
                "Workshop",
                new Resources { Wood = 350 },
                10,
                new IResources[] { new Resources { Wood = -5, Instruments = 3} })
            {
                Builders = 2,
                BuildingTime = TimeSpan.FromHours(6),
                LifePoints = 500,
                Acceleration = Acceleration,
                RequiredResearches = new[] { Researches.Current.Root, },
            };



        private BuildingPrototypes()
        {
            SetUpgrades();
        }



        private void SetUpgrades()
        {
            Plain.Upgrades = new[]
            {
                UndergroundHouse,
                WoodenHouse,
                Mill,
                Barracks,
                HeavyBarracks,
                CoalMine,
                Bakery,
                SpiritHouse,
                Field,
                Workshop,
            };
            
            Forest.Upgrades = new[]
            {
                Sawmill,
                HuntersShack,
            };

            UndergroundHouse.Upgrades = new[] {WoodenHouse};
        }
    }
}