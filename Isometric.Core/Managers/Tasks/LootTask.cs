using System;
using System.Runtime.InteropServices;
using Isometric.Core.Common;
using Isometric.Core.Vectors;

namespace Isometric.Core.Managers.Tasks
{
    public class LootTask : ArmyTask
    {
        public MovingTask CurrentMovingTask { get; private set; }
        
        public DestroyingTask CurrentDestroyingTask { get; private set; }
        
        public int Range { get; }
        
        public Player Enemy { get; }

        
        
        public LootTask(Army army, Vector to, int range)
        {
            CurrentMovingTask = new MovingTask {Army = army, To = to,};
            Army = army;
            Range = range;
            Enemy = army.World.GetBuilding(to).Owner;
        }
        
        
        
        public override bool Tick(TimeSpan deltaTime)
        {
            if (!(CurrentMovingTask.Tick(deltaTime) && (CurrentDestroyingTask?.Tick(deltaTime) ?? true)))
            {
                return false;
            }

            var nearestEnemy = 
                Army.World.FindNearestBuilding(
                    Army.Position, 
                    Range, 
                    b => b?.Owner == Enemy 
                         && Army.Constants.DestroyedBuilding != b?.Prototype);
            
            if (nearestEnemy == null)
            {
                return true;
            }

            if (nearestEnemy.Position == Army.Position)
            {
                CurrentDestroyingTask = new DestroyingTask {Army = Army,};
            }
            else
            {
                CurrentMovingTask = new MovingTask {Army = Army, To = nearestEnemy.Position,};
            }

            return Tick(deltaTime);
        }
    }
}