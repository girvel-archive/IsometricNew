using System;
using Isometric.Core.AreaGeneration;

namespace Isometric.Core
{
    public class GameConstants
    {
        public TimeSpan PeopleGenerationSize { get; set; } = TimeSpan.FromDays(15);

        public IResources ZeroResources { get; set; }

        public IResources PersonConsumption { get; set; }

        public IResources StartResources { get; set; }

        public TimeSpan PersonConsumptionPeriod { get; set; }

        public float HungerK { get; set; } = 0.5f;

        public float BuildingDefaultLootK { get; set; } = 0.8f;

        public Building DestroyedBuilding { get; set; }

        public IAreaModifier PlayerStartVillageModifier { get; set; }
        
        public int StartPeople { get; set; }
    }
}