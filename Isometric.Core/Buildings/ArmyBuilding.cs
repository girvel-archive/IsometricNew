using System;
using Isometric.Core.Common;

namespace Isometric.Core.Buildings
{
    public class ArmyBuilding : Building
    {
        public Army ArmyPrototype { get; set; }

        public TimeSpan ArmyCreationTime { get; set; }

        public new ArmyBuilding Prototype
        {
            get => (ArmyBuilding) base.Prototype;
            set => base.Prototype = value;
        }



        /// <summary>
        /// testing ctor
        /// </summary>
        public ArmyBuilding() { }

        public ArmyBuilding(string name, IResources price,Army armyPrototype, TimeSpan armyCreationTime) 
            : base(name, price)
        {
            ArmyPrototype = armyPrototype;
            ArmyCreationTime = armyCreationTime;
        }


        public override void Tick(TimeSpan deltaTime)
        {
            base.Tick(deltaTime);

            if (TotalPeople >= ArmyPrototype.RequiredPeople && Owner.Resources.Enough(ArmyPrototype.Price))
            {
                ArmyCreationTime -= deltaTime.Multiple(ArePeopleHungry ? Constants.HungerK : 1.0f);

                while (ArmyCreationTime <= TimeSpan.Zero)
                {
                    if (TotalPeople < ArmyPrototype.RequiredPeople 
                        || !Owner.Resources.Enough(ArmyPrototype.Price))
                    {
                        ArmyCreationTime = Prototype.ArmyCreationTime;
                        break;
                    }

                    Army.CreateByPrototype(ArmyPrototype, Owner, World, Position, Constants);

                    ArmyCreationTime += Prototype.ArmyCreationTime;
                    FreePeople -= ArmyPrototype.RequiredPeople;
                    Owner.Resources = Owner.Resources.Substracted(ArmyPrototype.Price);
                }
            }
        }
    }
}