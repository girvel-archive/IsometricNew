using System;

namespace Isometric.Dtos
{
    public class BuildingAreaDto
    {
        public string Name, OwnerName;

        public TimeSpan BuildingTime;

        public bool IsThereArmy, ArePeopleHungry;
    }
}