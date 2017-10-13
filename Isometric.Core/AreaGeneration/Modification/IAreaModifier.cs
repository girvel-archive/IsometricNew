using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration.Modification
{
    public interface IAreaModifier
    {
        void Modify(World world, Vector areaPosition, Player player);
    }
}