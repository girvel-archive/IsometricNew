using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public class ArmyModifier : IAreaModifier
    {
        public Army ArmyPrototype { get; set; }
        
        public Vector Position { get; set; }
        
        
        
        public void Modify(World world, Vector areaPosition, Player player)
        {
            Army.CreateByPrototype(ArmyPrototype, player, world, Position, world.Constants);
        }
    }
}