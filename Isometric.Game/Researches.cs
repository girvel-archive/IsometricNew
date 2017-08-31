using Isometric.Core;

namespace Isometric.Game
{
    internal class Researches
    {
        private static Researches _current;
        public static Researches Current => _current ?? (_current = new Researches());



        public Research 
            Root, 
            Agriculture,
            WoodenArchitecture,
            Bakeries,
            BigFamilies,
            HeavyInfantry,
            CoalMines;



        private Researches()
        {
            Root = new Research(
                "Instruments",
                1,
                new[]
                {
                    Agriculture = new Research(
                        "Agriculture",
                        1,
                        new[]
                        {
                            WoodenArchitecture = new Research(
                                "Wooden architecture",
                                1,
                                new[]
                                {
                                    Bakeries = new Research(
                                        "Bakeries",
                                        1,
                                        new Research[0]), 
                                }),
                            BigFamilies = new Research(
                                "Big families",
                                1,
                                new Research[0]),
                        HeavyInfantry = new Research(
                            "Heavy infantry",
                            1,
                            new Research[0]),
                        }),
                    CoalMines = new Research(
                        "Coal mines",
                        1,
                        new Research[0]),
                });
        }
    }
}