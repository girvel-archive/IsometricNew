using System;
using Isometric.Core.Common;

namespace Isometric.Core.Buildings
{
    public class PeriodIncomeBuilding : IncomeBuilding
    {
        /// <summary>
        /// testing ctor
        /// </summary>
        public PeriodIncomeBuilding() { }

        public PeriodIncomeBuilding(string name, IResources price, int workers, IResources[] incomes)
            : base(name, price, workers, incomes) { }



        protected override IResources GetIncome(TimeSpan deltaTime)
        {
            IncomePeriod -= deltaTime;

            LastIncome = Constants.ZeroResources;

            while (IncomePeriod <= TimeSpan.Zero)
            {
                IncomePeriod += Prototype.IncomePeriod;

                LastIncome = LastIncome.Added(base.GetIncome(Prototype.IncomePeriod));
            }

            return LastIncome;
        }
    }
}