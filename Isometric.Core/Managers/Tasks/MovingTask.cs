using System;
using Isometric.Core.Common;
using Isometric.Core.Vectors;

namespace Isometric.Core.Managers.Tasks
{
    public class MovingTask : ArmyTask
    {
        public Vector To;

        public override bool Tick(TimeSpan deltaTime)
        {
            Army.MovingTime -= deltaTime.Multiple(Army.IsHungry ? Army.Constants.HungerK : 1);

            while (Army.MovingTime <= TimeSpan.Zero)
            {
                Army.MovingTime += Army.Prototype.MovingTime;

                if (Army.Position.X != To.X)
                {
                    Army.Position += (To.X > Army.Position.X ? 1 : -1) * new Vector(1, 0);
                }
                else if (Army.Position.Y != To.Y)
                {
                    Army.Position += (To.Y > Army.Position.Y ? 1 : -1) * new Vector(0, 1);
                }
                else
                {
                    Army.MovingTime = Army.Prototype.MovingTime;
                    return true;
                }
            }

            return false;
        }
    }
}