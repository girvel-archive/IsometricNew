using System;
using Isometric.Core;

namespace Isometric.Game
{
    public class Armies
    {
        private static Armies _current;
        public static Armies Current = _current ?? (_current = new Armies());



        public Army
            Infantry = new Army(
                "Infantry",
                10,
                TimeSpan.FromSeconds(3),
                10,
                Resources.Zero,
                new Resources {Food = 10})
            {
                Damage = 10,
                Armor = ArmorType.Medium,
            },
            HeavyInfantry = new Army(
                "Heavy infantry",
                15,
                TimeSpan.FromSeconds(6),
                10,
                Resources.Zero,
                new Resources {Food = 10})
            {
                Armor = ArmorType.Heavy,
                BonusDamageArmorType = ArmorType.Building,
                BonusDamage = 10,
            };
    }
}