namespace Isometric.Core.Managers
{
    public class VisionManager
    {
        public bool IsAreaOpened(Player player, Area area)
        {
            return area == player.Area;
        }
    }
}