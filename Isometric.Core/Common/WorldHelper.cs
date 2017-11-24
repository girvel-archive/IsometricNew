using System;
using Isometric.Core.Vectors;

namespace Isometric.Core.Common
{
    public static class WorldHelper
    {
        public static Building FindNearestBuilding(
            this World world, Vector position, int range, Predicate<Building> condition)
        {
            for (var i = 0; i <= range; i++)
            {
                for (var dx = -i; dx < i; dx++)
                for (var dy = -i; dy < i; dy++)
                {
                    var currentBuilding = world.GetBuilding(position + new Vector(dx, dy));
                    if (condition(currentBuilding))
                    {
                        return currentBuilding;
                    }
                }
            }

            return null;
        }
    }
}