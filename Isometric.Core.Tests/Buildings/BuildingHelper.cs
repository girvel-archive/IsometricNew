using Isometric.Core.Buildings;

namespace Isometric.Core.Tests.Buildings
{
    public static class BuildingHelper
    {
        public static void MakeBuildingSafeForTick(this Building building)
        {
            building.Constants = building.Constants ?? new GameConstants();
            building.Owner = building.Owner ?? new Player();
        }

        public static void MakeBuildingSafeForTick(this ArmyBuilding building)
        {
            MakeBuildingSafeForTick((Building) building);

            building.World = building.World ?? new World {AreaWidth = 1};
        }
    }
}