using System;
using System.Linq;
using Isometric.Core.Common;
using Isometric.Core.Time;
using Isometric.Core.Vectors;

namespace Isometric.Core
{
    public class Army : ITimeObject, IMilitaryObject
    {
        public GameConstants Constants { get; set; }

        public string Name { get; set; }

        public int RequiredPeople { get; set; }

        public Army Prototype { get; set; }

        public Player Owner { get; set; }

        public Vector Position
        {
            get => _position;
            set
            {
                var enemy = World.GetBuilding(value).Armies.FirstOrDefault(a => a.Owner != Owner);
                if (enemy != null && !Attack(enemy))
                {
                    return;
                }

                var oldPosition = new Vector();
                var oldIndex = 0;
                var oldBuilding = World.GetBuilding(_position);
                
                if (oldBuilding != null)
                {
                    oldPosition = _position;
                    oldIndex = oldBuilding.Armies.IndexOf(this);
                }

                World.GetBuilding(_position)?.Armies.Remove(this);
                
                _position = value;

                if (!World.GetBuilding(_position).Armies.Contains(this))
                {
                    World.GetBuilding(_position).Armies.Add(this);
                }

                if (oldBuilding != null)
                {
                    OnPositionChanged(oldPosition, oldIndex);
                }
            }
        }
        private Vector _position;

        public TimeSpan MovingTime { get; set; }

        public TimeSpan AttackTime { get; set; }
        
        public World World { get; set; }


        #region IMilitaryObject

        public virtual int LifePoints { get; set; }

        public virtual int Damage { get; set; }

        public virtual int BonusDamage { get; set; }

        public ArmorType Armor { get; set; }

        public ArmorType BonusDamageArmorType { get; set; }

        public virtual int GetDamageFor(IMilitaryObject enemy)
        {
            return Damage + (enemy.Armor == BonusDamageArmorType ? BonusDamage : 0);
        }

        public IResources Loot => Constants.ZeroResources;

        public void Destroy(IMilitaryObject destroyer)
        {
            End();
        }

        #endregion



        public IResources Price { get; set; }

        public IResources Consumption { get; set; }

        public TimeSpan ConsumptionPeriod { get; set; } = TimeSpan.FromMinutes(1);

        public bool IsHungry { get; set; }




        public Action<IMilitaryObject> OnDestroyedObject { get; set; }

        public PositionChangedAction OnPositionChanged { get; set; } = (v, index) => { };

        public delegate void PositionChangedAction(Vector oldPosition, int oldIndex);



        /// <summary>
        /// testing ctor
        /// </summary>
        public Army() { }

        public Army(string name, int lifePoints, TimeSpan movingTime, int requiredPeople, IResources price, IResources consumption)
        {
            Name = name;
            LifePoints = lifePoints;
            MovingTime = movingTime;
            RequiredPeople = requiredPeople;
            Price = price;
            Consumption = consumption;
        }

        public static Army CreateByPrototype(Army prototype, Player owner, World world, Vector position, GameConstants constants)
        {
            var result = (Army) prototype.MemberwiseClone();

            result.Prototype = prototype;
            result.Owner = owner;
            result.World = world;
            result.Position = position;
            result.Constants = constants;

            result.Owner?.OnArmyCreated?.Invoke(result);

            return result;
        }

        private void End()
        {
            World.GetBuilding(Position).Armies.Remove(this);
        }



        public virtual void Tick(TimeSpan deltaTime)
        {
            // consumption

            if (!Consumption.IsZero())
            {
                var currentConsumption = Consumption.Multiplied((float)deltaTime.Ticks / ConsumptionPeriod.Ticks);

                Owner.Resources =
                    (IsHungry = !Owner.Resources.Enough(currentConsumption))
                        ? Constants.ZeroResources
                        : Owner.Resources.Substracted(currentConsumption);
            }
        }



        public bool Attack(IMilitaryObject enemy)
        {
            enemy.LifePoints -= GetDamageFor(enemy);
            LifePoints -= enemy.GetDamageFor(this);

            if (LifePoints <= 0)
            {
                Destroy(enemy);
            }

            if (enemy.LifePoints <= 0)
            {
                Owner.Resources = Owner.Resources.Added(enemy.Loot);
                enemy.Destroy(this);
                OnDestroyedObject?.Invoke(enemy);
            }

            return LifePoints > 0 && enemy.LifePoints <= 0;
        }
    }
}