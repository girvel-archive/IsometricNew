using Isometric.Core.Vectors;

namespace Isometric.Dtos
{
    public class VisionDto
    {
        public BuildingAreaDto[,] Buildings { get; set; }
        
        public Vector Position { get; set; }
    }
}