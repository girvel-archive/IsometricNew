using System;

namespace Isometric.Core
{
    public interface IResources : ICloneable
    {
        float[] ResourcesArray { get; }

        float this[int index] { get; set; }

        int ResourceTypesNumber { get; }

        bool Enough(IResources Resources);
    }
}