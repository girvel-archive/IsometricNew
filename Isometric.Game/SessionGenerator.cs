using System;
using ChanceList;
using Isometric.Core;
using Isometric.Core.AreaGeneration;
using Isometric.Core.Buildings;
using Isometric.Core.Vectors;

namespace Isometric.Game
{
    public class SessionGenerator
    {
        public Session Generate()
        {
            var constants = new GameConstants
            {
                ZeroResources = Resources.Zero,
                StartResources = new Resources {Wood = 1000},
                PeopleGenerationSize = TimeSpan.FromDays(30),
                DestroyedBuilding = BuildingPrototypes.Current.Plain,
                PlayerStartVillageModifier = new VillageModifier(BuildingPrototypes.Current.UndergroundHouse, 6, new Vector(4, 4)),
                StartPeople = 30,
            };
            
            var world = new World(
                16,
                128,
                new IAreaGenerator[]
                {
                    new ComplexAreaGenerator(
                        new RandomGenerator(
                            new ChanceCollection<Building>
                            {
                                new ChancePair<Building>(3, BuildingPrototypes.Current.Plain),
                                new ChancePair<Building>(3, BuildingPrototypes.Current.Forest),
                            }),
                        new IAreaModifier[]
                        {
                            // village
                            new VillageModifier(BuildingPrototypes.Current.WoodenHouse, 12, new Vector(12, 8)), 
                        })
                },
                new Random().Next(),
                constants);

            return new Session(
                world, 
                BuildingPrototypes.Current.GetAllPrototypes(), 
                Researches.Current.Root);
        }
    }
}
