using System;

namespace Isometric.Core.Buildings
{
    public class ScienceBuilding : WorkerBuilding
    {
        public float ResearchIncome { get; set; }

        public TimeSpan IncomePeriod = TimeSpan.FromMinutes(1);

        public new ScienceBuilding Prototype
        {
            get { return (ScienceBuilding) base.Prototype; }
            set { base.Prototype = value; }
        }



        /// <summary>
        /// testing ctor
        /// </summary>
        public ScienceBuilding() { }

        public ScienceBuilding(string name, IResources price, float researchIncome, int workers) 
            : base(name, price, workers)
        {
            ResearchIncome = researchIncome;
        }


        public override void Tick(TimeSpan deltaTime)
        {
            base.Tick(deltaTime);

            if (Owner.CurrentResearch != null)
            {
                Owner.CurrentResearchPoints
                    += ResearchIncome
                       * Efficiency
                       * ((float) deltaTime.Ticks / IncomePeriod.Ticks);
            }
        }
    }
}