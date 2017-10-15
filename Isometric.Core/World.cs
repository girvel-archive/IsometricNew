using System;
using Isometric.Core.AreaGeneration;
using Isometric.Core.Common;
using Isometric.Core.Time;
using Isometric.Core.Vectors;

namespace Isometric.Core
{
    public class World : ITimeObject
    {
        public GameConstants Constants { get; set; }

        public Action<Player> OnPlayerCreate { get; set; } = p => { };

        public Area[,] Landscape { get; set; }

        public virtual int AreaWidth { get; set; }

        public IAreaGenerator[] Generators { get; set; }

        public int Seed { get; set; }

        public virtual Vector Size => 
            new Vector(
                Landscape.GetLength(0) * AreaWidth, 
                Landscape.GetLength(1) * AreaWidth);



        /// <summary>
        /// testing ctor
        /// </summary>
        public World() { }

        public World(int areaWidth, int worldWidth, IAreaGenerator[] generators, int seed, GameConstants constants)
        {
            AreaWidth = areaWidth;
            Landscape = new Area[worldWidth, worldWidth];
            Generators = generators;
            Seed = seed;
            Constants = constants;
        }



        public void Tick(TimeSpan timeSpan)
        {
            foreach (var area in Landscape)
            {
                area?.Tick(timeSpan);
            }
        }

        public void GenerateArea(int x, int y)
        {
            Generators[new Random(Seed).Next(Generators.Length)]
                .GenerateArea(this, new Vector(x, y), Constants.NeutralPlayer);
        }

        public bool TryUpgrade(Vector position, Building to)
        {
            var from = GetBuilding(position);

            if (!from.IsUpgradePossible(to))
            {
                return false;
            }

            from.Owner.Resources = from.Owner.Resources.Substracted(to.Price);

            SetBuilding(
                position, 
                Building.CreateByPrototype(
                    to,
                    from.Owner,
                    from.World,
                    position,
                    from.Constants,
                    from));

            return true;
        }



        public Area GetArea(Vector areaPosition)
        {
            return Landscape[areaPosition.X, areaPosition.Y];
        }

        public void SetArea(Vector areaPosition, Area value)
        {
            Landscape[areaPosition.X, areaPosition.Y] = value;
        }
        
        public virtual Building GetBuilding(Vector position)
        {
            return GetArea(position.GetAreaPosition(AreaWidth))?[position.GetLocalPosition(AreaWidth)];
        }

        public void SetBuilding(Vector position, Building value)
        {
            GetArea(position.GetAreaPosition(AreaWidth))[position.GetLocalPosition(AreaWidth)] = value;
        }
    }
}