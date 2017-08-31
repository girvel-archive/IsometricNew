using System;
using Isometric.Core.Common;
using Isometric.Core.Time;
using Isometric.Core.Vectors;

namespace Isometric.Core
{
    public class Area : ITimeObject
    {
        public Building this[Vector position]
        {
            get => this[position.X, position.Y];
            set => this[position.X, position.Y] = value;
        }

        public Building this[int x, int y]
        {
            get => Buildings[x, y];
            set => Buildings[x, y] = value;
        }

        public Building[,] Buildings { get; set; }

        public World World { get; set; }

        public Vector Position { get; set; }



        /// <summary>
        /// testing ctor
        /// </summary>
        public Area() { }

        public Area(World world, Vector position)
        {
            Position = position;
            World = world;
            Buildings = new Building[World.AreaWidth, World.AreaWidth];
        }


        public virtual void Tick(TimeSpan deltaTime)
        {
            foreach (var b in Buildings)
            {
                b.Tick(deltaTime);
            }
        }
    }
}