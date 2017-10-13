using Isometric.Core.AreaGeneration.Modification;
using Isometric.Core.Vectors;

namespace Isometric.Core.AreaGeneration
{
    public class ComplexAreaGenerator : IAreaGenerator
    {
        public IAreaGenerator Generator { get; set; }

        public IAreaModifier[] Modifiers { get; set; }



        /// <summary>
        /// testing ctor
        /// </summary>
        public ComplexAreaGenerator() { }

        public ComplexAreaGenerator(IAreaGenerator generator, IAreaModifier[] modifiers)
        {
            Generator = generator;
            Modifiers = modifiers;
        }



        public void GenerateArea(World world, Vector position, Player owner)
        {
            Generator.GenerateArea(world, position, owner);

            foreach (var modifier in Modifiers)
            {
                modifier.Modify(world, position, owner);
            }
        }
    }
}