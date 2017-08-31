using System;

namespace Isometric.Core.Time
{
    public interface ITimeObject
    {
        void Tick(TimeSpan deltaTime);
    }
}