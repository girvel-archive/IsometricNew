using ChanceList;
using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public class RandomGenerator : IAreaGenerator
    {
        public ChanceCollection<Building> ChanceCollection;



        public RandomGenerator(ChanceCollection<Building> chanceCollection)
        {
            ChanceCollection = chanceCollection;
        }


        public void GenerateArea(World world, Vector position)
        {
            var area = new Area(world, position);

            for (var x = 0; x < area.Buildings.GetLength(0); x++)
            for (var y = 0; y < area.Buildings.GetLength(1); y++)
            {
                area.Buildings[x, y] 
                    = Building.CreateByPrototype(
                        ChanceCollection.Get(), 
                        Player.Neutral, 
                        world, 
                        position * world.AreaWidth + new Vector(x, y), 
                        world.Constants);
            }

            world.SetArea(position, area);
        }
    }
}