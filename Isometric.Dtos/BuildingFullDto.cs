using System;

namespace Isometric.Dtos
{
    public class BuildingFullDto
    {
        public string OwnerName, Name, CreatingArmy;

        public int FreePeople, Builders, Workers, MaxWorkers, MaxBuilders, PeopleForArmy, ArmyQueueSize;

        public float[] Income, LastIncome, ArmyPrice;

        public string[] Armies;

        public bool IsWorkerBuilding, IsIncomeBuilding, IsArmyBuilding, IsFinished;

        public TimeSpan ArmyCreationTime, ArmyCreationTimeMax;
    }
}