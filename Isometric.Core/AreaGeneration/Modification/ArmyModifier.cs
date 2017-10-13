using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration.Modification
{
    public class ArmyModifier : IAreaModifier
    {
        public Army ArmyPrototype { get; set; }
        
        public Vector RelativePosition { get; set; }



        public ArmyModifier(Army prototype, Vector relativePosition)
        {
            ArmyPrototype = prototype;
            RelativePosition = relativePosition;
        }
        
        public void Modify(World world, Vector areaPosition, Player player)
        {
            Army.CreateByPrototype(
                ArmyPrototype, 
                player, 
                world, 
                areaPosition * world.AreaWidth + RelativePosition, 
                world.Constants);
        }
    }
}