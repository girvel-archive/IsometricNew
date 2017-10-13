using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration.Modification
{
    public class ComplexModifier : IAreaModifier
    {
        public IAreaModifier[] Modifiers { get; set; }

        public ComplexModifier(params IAreaModifier[] modifiers)
        {
            Modifiers = modifiers;
        }
        
        public void Modify(World world, Vector areaPosition, Player player)
        {
            foreach (var modifier in Modifiers)
            {
                modifier.Modify(world, areaPosition, player);
            }
        }
    }
}