using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public interface IAreaGenerator
    {
        void GenerateArea(World world, Vector position, Player owner);
    }
}