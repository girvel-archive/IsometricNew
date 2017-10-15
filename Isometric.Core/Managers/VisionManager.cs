using System;
using System.Linq;
using Isometric.Core.Vectors;

namespace Isometric.Core.Managers
{
    public class VisionManager
    {
        public int VisionDefaultRange = 5;
        
        public bool IsAreaOpened(Player player, Area area)
        {
            return area == player.Area;
        }

        public bool IsVisible(World world, Player player, Vector absolutePosition)
        {
            for (var x = -VisionDefaultRange; x <= VisionDefaultRange; x++)
            for (var y = -VisionDefaultRange; y <= VisionDefaultRange; y++)
            {
                var deltaPosition = new Vector(x, y);

                if (deltaPosition.Magnitude <= VisionDefaultRange)
                {
                    var currentBuilding = world.GetBuilding(absolutePosition + deltaPosition);
                    if (currentBuilding != null
                        && (currentBuilding.Owner == player
                            || currentBuilding.Armies.Any(a => a.Owner == player)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Vision GetVision(World world, Player player)
        {
            var position = new Vector(
                Math.Max(0, player.OwnBuildings.Min(b => b.Position.X) - VisionDefaultRange), 
                Math.Max(0, player.OwnBuildings.Min(b => b.Position.Y) - VisionDefaultRange));

            var result = new Vision
            {
                Buildings = new Building[
                    Math.Min(
                        world.Size.X, 
                        player.OwnBuildings.Max(b => b.Position.X) + VisionDefaultRange + 1) 
                    - position.X,
                    Math.Min(
                        world.Size.Y, 
                        player.OwnBuildings.Max(b => b.Position.Y) + VisionDefaultRange + 1) 
                    - position.Y],
                Position = position,
            };
            
            foreach (var building in player.OwnBuildings)
            {
                for (var x = Math.Max(-building.Position.X, -VisionDefaultRange); 
                    x <= Math.Min((world.Size - building.Position).X - 1, VisionDefaultRange); 
                    x++)
                for (var y = Math.Max(-building.Position.Y, -VisionDefaultRange); 
                    y <= Math.Min((world.Size - building.Position).Y - 1, VisionDefaultRange); 
                    y++)
                {
                    var deltaPosition = new Vector(x, y);

                    if (deltaPosition.Magnitude <= VisionDefaultRange)
                    {
                        var absolutePosition = building.Position + deltaPosition;
                        var visionPosition = absolutePosition - position;

                        result.Buildings[visionPosition.X, visionPosition.Y] = world.GetBuilding(absolutePosition);
                    }
                }
            }

            return result;
        }
    }
}