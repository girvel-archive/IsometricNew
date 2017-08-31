using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public interface IAreaModifier
    {
        void Modify(World world, Vector areaPosition, Player player);
    }
}