using Isometric.Core.Common;

namespace Isometric.Core
{
    public struct DefaultResources : IResources
    {
        private float[] _resourcesArray;

        public float this[int index]
        {
            get { return ResourcesArray[index]; }
            set { ResourcesArray[index] = value; }
        }

        public float[] ResourcesArray => _resourcesArray ?? (_resourcesArray = new float[ResourceTypesNumber]);

        public int ResourceTypesNumber => 3;



        public DefaultResources(float r1, float r2 = 0, float r3 = 0)
        {
            _resourcesArray = new[] { r1, r2, r3 };
        }

        public bool Enough(IResources price)
        {
            return ResourcesHelper.Enough(this, price);
        }

        public object Clone() => new DefaultResources {_resourcesArray = (float[]) ResourcesArray.Clone()};
    }
}