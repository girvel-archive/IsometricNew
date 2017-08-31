using Isometric.Core.Vectors;

namespace Isometric.Core.Common
{
    public static class VectorHelper
    {
        public static Vector GetAreaPosition(this Vector absolutePosition, int areaWidth) 
            => new Vector(
                absolutePosition.X / areaWidth,
                absolutePosition.Y / areaWidth);
        
        public static Vector GetLocalPosition(this Vector absolutePosition, int areaWidth) 
            => new Vector(
                absolutePosition.X % areaWidth,
                absolutePosition.Y % areaWidth);
    }
}