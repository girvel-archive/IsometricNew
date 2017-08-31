using System;

namespace Isometric.Dtos
{
    public class UpgradeDto
    {
        public string Name;

        public float[] Price;

        public TimeSpan Time;

        public UpgradePossibility Possibility;

        public string[] RequiredResearches;
    }
}