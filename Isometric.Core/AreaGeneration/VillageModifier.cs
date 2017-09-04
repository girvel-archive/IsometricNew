using System;
using System.Linq;
using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public class VillageModifier : IAreaModifier
    {
        public Building BuildingPrototype { get; set; }

        public int BuildingsNumber { get; set; }

        public Vector Position { get; set; }



        public VillageModifier(Building buildingPrototype, int buildingsNumber, Vector position)
        {
            BuildingPrototype = buildingPrototype;
            BuildingsNumber = buildingsNumber;
            Position = position;
        }



        public void Modify(World world, Vector areaPosition, Player player)
        {
            if (Math.Pow(world.AreaWidth, 2) < BuildingsNumber)
            {
                throw new InvalidOperationException("Area is too small");
            }

            var random = new Random();
            var area = world.GetArea(areaPosition);
            var villageSize = (int) Math.Ceiling(Math.Sqrt(BuildingsNumber));

            if (
                new[] {Position.X, Position.Y, world.AreaWidth - Position.X, world.AreaWidth - Position.Y}.Any(
                    p => p < villageSize))
            {
                throw new InvalidOperationException("Village is too big for this position and area");
            }

            for (var i = 0; i < BuildingsNumber; i++)
            {
                Vector currentPosition;
                do
                {
                    currentPosition = Position + new Vector(
                        random.Next(-villageSize, villageSize) / 2,
                        random.Next(-villageSize, villageSize) / 2);
                } while (area[currentPosition].Prototype == BuildingPrototype);

                var building 
                    = area[currentPosition] 
                    = Building.CreateByPrototype(
                        BuildingPrototype, 
                        player, 
                        world, 
                        areaPosition * world.AreaWidth + currentPosition, 
                        world.Constants);

                //building.Finished = true;
            }
        }
    }
}