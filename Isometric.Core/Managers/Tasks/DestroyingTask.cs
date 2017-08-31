using System;

namespace Isometric.Core.Managers.Tasks
{
    public class DestroyingTask : ArmyTask
    {
        public override bool Tick(TimeSpan deltaTime)
        {
            Army.AttackTime -= deltaTime;

            while (Army.AttackTime <= TimeSpan.Zero)
            {
                Army.AttackTime += Army.Prototype.AttackTime;

                if (Army.Attack(Army.World.GetBuilding(Army.Position)))
                {
                    Army.AttackTime = Army.Prototype.AttackTime;
                    return true;
                }
            }

            return false;
        }
    }
}