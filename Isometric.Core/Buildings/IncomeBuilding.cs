using System;
using System.Collections.Generic;
using System.Linq;
using Isometric.Core.Common;

namespace Isometric.Core.Buildings
{
    public class IncomeBuilding : WorkerBuilding
    {
        public TimeSpan IncomePeriod = TimeSpan.FromMinutes(1);

        public IResources[] Incomes { get; set; }

        public new IncomeBuilding Prototype
        {
            get => (IncomeBuilding) base.Prototype;
            set => base.Prototype = value;
        }

        public AccelerationCollection Acceleration { get; set; }

        public int PrefferedIncomeIndex { get; set; } = -1;

        public IResources LastIncome { get; set; }

        public int LastIncomeIndex { get; set; }



        /// <summary>
        /// testing ctor
        /// </summary>
        public IncomeBuilding() { }

        public IncomeBuilding(string name, IResources price, int workers, IResources[] incomes) 
            : base(name, price, workers)
        {
            Incomes = incomes;
        }



        public override void Tick(TimeSpan deltaTime)
        {
            base.Tick(deltaTime);

            if (Finished)
            {
                // owner.resources can be changed in GetAcceleration()
                Owner.Resources =
                    GetIncome(deltaTime)
                        .Multiplied(GetAcceleration() * Efficiency)
                        .Added(Owner.Resources);

                LastIncome = LastIncome.Multiplied(GetAcceleration() * Efficiency);
            }
        }



        protected virtual float GetAcceleration()
        {
            if (!(Acceleration?.Accelerations?.Any() ?? false))
            {
                return 1.0f;
            }

            float endAcceleration = 1;

            IResources endPrice = null;

            foreach (var a in Acceleration.Accelerations)
            {
                var currentEnoughK = Owner.Resources.GetEnoughCoefficient(a.RequiredResources);
                var currentAcceleration = a.Acceleration * currentEnoughK;

                if (currentAcceleration > endAcceleration)
                {
                    endAcceleration = currentAcceleration;
                    endPrice = a.RequiredResources.Multiplied(currentEnoughK);
                }
            }

            if (endPrice != null)
            {
                Owner.Resources = Owner.Resources.Substracted(endPrice);
            }
            
            return endAcceleration;
        }

        protected virtual IResources GetIncome(TimeSpan deltaTime)
        {
            LastIncome = Constants.ZeroResources;
            if (PrefferedIncomeIndex > 0)
            {
                LastIncome = Incomes[PrefferedIncomeIndex];
            }
            else
            {
                var i = 0;
                foreach (var currentIncome in Incomes)
                {
                    var k = Owner.Resources.GetEnoughCoefficient(currentIncome.Inverted());

                    if (k != 0)
                    {
                        LastIncome = currentIncome.Multiplied(k);
                        LastIncomeIndex = i;
                        break;
                    }
                    i++;
                }
            }

            return LastIncome.Multiplied((float) (deltaTime.TotalMilliseconds / Prototype.IncomePeriod.TotalMilliseconds));
        }



        public class AccelerationData
        {
            public float Acceleration { get; set; }

            public IResources RequiredResources { get; set; }



            public AccelerationData(float acceleration, IResources requiredResources)
            {
                Acceleration = acceleration;
                RequiredResources = requiredResources;
            }
        }

        public class AccelerationCollection
        {
            public List<AccelerationData> Accelerations 
                = new List<AccelerationData>();
            
            public float this[IResources key]
            {
                set
                {
                    var a = Accelerations.FirstOrDefault(e => e.RequiredResources.Equals(key));

                    if (a == null)
                    {
                        Accelerations.Add(a = new AccelerationData(0, key));
                    }

                    a.Acceleration = value;
                }
            }
        }
    }
}