using System;
using System.Collections.Generic;
using System.Linq;
using Isometric.Core.Time;
using Isometric.Core.Vectors;

namespace Isometric.Core
{
    public class Player : ITimeObject
    {
        public static readonly Player Neutral = new Player();



        public GameConstants Constants { get; set; }

        public IResources Resources { get; set; }

        public Area Area { get; set; }

        public List<Research> ResearchedTechnologies { get; set; } = new List<Research>();

        public Research CurrentResearch { get; set; }

        public float CurrentResearchPoints { get; set; }

        public float BirthrateK { get; set; } = 1F;



        public int TotalPeople { get; set; }

        public int MaxPeople { get; set; }

        public List<Building> OwnBuildings { get; set; } 
            = new List<Building>();



        public Action<Building> OnBuildingBegin = b => { };

        public Action<Building> OnBuildingEnd = b => { };

        public Action<Army> OnArmyCreated = b => { };



        /// <summary>
        /// testing ctor
        /// </summary>
        public Player() { }

        public Player(Area area, GameConstants constants)
        {
            Area = area;
            Constants = constants;
        }

        public static Player CreateForWorld(World world)
        {
            var emptyAreas = world.Landscape.Cast<Area>().Count(a => a == null);

            if (emptyAreas == 0)
            {
                throw new ArgumentException("World's landscape is full");
            }

            var playerAreaIndex = new Random(world.Seed).Next(emptyAreas);

            int x = playerAreaIndex % world.Landscape.GetLength(0),
                y = playerAreaIndex / world.Landscape.GetLength(0);
            
            world.GenerateArea(x, y);

            var result = new Player(world.Landscape[x, y], world.Constants)
            {
                Resources = world.Constants.StartResources
            }; 
            
            var b = result.Area[0, 0];
            b.FreePeople = world.Constants.StartPeople;
            b.Owner = result;

            world.OnPlayerCreate(result);
            world.Constants.PlayerStartVillageModifier?.Modify(world, new Vector(x, y), result);

            Army.CreateByPrototype(
                new Army(
                    "Infantry", 
                    20, 
                    TimeSpan.FromSeconds(1), 
                    10, 
                    world.Constants.ZeroResources, 
                    world.Constants.ZeroResources)
                {
                    Damage = 5,
                    BonusDamage = 5,
                    BonusDamageArmorType = ArmorType.Building,
                    Armor = ArmorType.Heavy,
                    AttackTime = TimeSpan.FromSeconds(15),
                }, 
                result, 
                result.Area.World, 
                b.Position, 
                world.Constants);

            return result;
        }



        public void Tick(TimeSpan deltaTime)
        {
            if (CurrentResearch != null && CurrentResearchPoints >= CurrentResearch.ResearchPointsRequired)
            {
                ResearchedTechnologies.Add(CurrentResearch);

                CurrentResearch.EndAction?.Invoke(this);

                CurrentResearchPoints -= CurrentResearch.ResearchPointsRequired;
                CurrentResearch = null;
            }
        }

        public bool BeginResearch(Research research, Research root)
        {
            if (!research.Possible(ResearchedTechnologies, root))
            {
                return false;
            }

            CurrentResearchPoints = 0;
            CurrentResearch = research;

            return true;
        }
    }
}