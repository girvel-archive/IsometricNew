using System;
using System.Linq;
using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public class RandomPositionModifier : IAreaModifier
    {
        public Building Prototype { get; set; }

        public int Number { get; set; }



        /// <summary>
        /// testing ctor
        /// </summary>
        public RandomPositionModifier() { }

        public RandomPositionModifier(Building prototype, int number)
        {
            Prototype = prototype;
            Number = number;
        }



        public void Modify(World world, Vector areaPosition, Player player)
        {
            var area = world.GetArea(areaPosition);

            if (area.Buildings.OfType<Building>().Count() < Number)
            {
                throw new InvalidOperationException("This world is too small for this modifier");
            }

            var random = new Random();

            for (var i = 0; i < Number; i++)
            {
                Vector currentReplacementPosition;
                do
                {
                    currentReplacementPosition
                        = new Vector(
                            random.Next(area.Buildings.GetLength(0)),
                            random.Next(area.Buildings.GetLength(1)));
                }
                while (area[currentReplacementPosition].Prototype == Prototype);

                area[currentReplacementPosition] 
                    = Building.CreateByPrototype(
                        Prototype,
                        player, 
                        world, 
                        areaPosition * world.AreaWidth + currentReplacementPosition, 
                        world.Constants, 
                        area[currentReplacementPosition]);
            }
        }
    }
}