using System;

namespace Isometric.Core.Managers.Tasks
{
    public abstract class ArmyTask
    {
        public Army Army { get; set; }

        public abstract bool Tick(TimeSpan deltaTime);
    }
}