using System;
using System.Collections.Generic;
using System.Linq;
using Isometric.Core.Common;
using Isometric.Core.Time;
using Isometric.Core.Vectors;

namespace Isometric.Core
{
    public delegate bool UpgradeCondition(Building from);

    public delegate void TickAction(TimeSpan deltaTime);

    public class Building : ITimeObject, IMilitaryObject
    {
        #region Private fields

        private bool _arePeopleHungry;
        private int _containsPeople;
        private Player _owner;
        private int _totalPeople;
        private int _freePeople;
        private int _builders;

        #endregion



        #region Building main variables

        public GameConstants Constants { get; set; }

        public Building Prototype { get; set; }

        public Player Owner
        {
            get => _owner;
            set
            {
                if (_owner != null)
                {
                    _owner.MaxPeople -= _containsPeople;
                    _owner.TotalPeople -= TotalPeople;
                }

                _owner = value;

                if (_owner != null)
                {
                    _owner.MaxPeople += _containsPeople;
                    _owner.TotalPeople += TotalPeople;
                }
            }
        }

        public World World { get; set; }

        public Vector Position { get; set; }

        public List<Army> Armies { get; set; } = new List<Army>();

        #endregion


        
        #region Prototype main variables

        public string Name { get; set; }

        public TimeSpan BuildingTime { get; set; }

        public bool Finished { get; set; }

        public Building[] Upgrades { get; set; } = {};

        public Research[] RequiredResearches { get; set; } = {};

        public IResources Price { get; set; }

        public TickAction TickAction { get; set; }

        #endregion

        

        #region People variables

        //public virtual int TotalPeople => FreePeople + Builders;

        protected TimeSpan PersonCreationTime { get; set; }

        public int TotalPeople
        {
            get => _totalPeople;
            protected set
            {
                lock (_totalPeopleLock)
                {
                    var oldValue = _totalPeople;
                    _totalPeople = value;

                    if (Owner != null)
                    {
                        Owner.TotalPeople += value - oldValue;
                    }
                }
            }
        }

        private readonly object _totalPeopleLock = new object();

        public int FreePeople
        {
            get { return _freePeople; }
            set
            {
                var oldValue = _freePeople;
                _freePeople = value;

                TotalPeople += value - oldValue;
            }
        }

        public int Builders
        {
            get { return _builders; }
            set
            {
                var oldValue = _builders;
                _builders = value;

                TotalPeople += value - oldValue;
            }
        }

        public bool ArePeopleHungry
        {
            get { return _arePeopleHungry; }
            set
            {
                var changed = _arePeopleHungry != value;

                _arePeopleHungry = value;

                if (changed)
                {
                    OnHungerChanged?.Invoke();
                }
            }
        }

        public int ContainsPeople
        {
            get { return _containsPeople; }
            set
            {
                var oldValue = _containsPeople;
                _containsPeople = value;

                if (Owner != null)
                {
                    Owner.MaxPeople += value - oldValue;
                }
            }
        }

        #endregion


        
        #region IMilitaryObject

        public int LifePoints { get; set; }

        public int Damage { get; set; }

        public virtual int GetDamageFor(IMilitaryObject enemy) => Damage;

        public virtual ArmorType Armor => ArmorType.Building;

        public virtual IResources Loot => Price.Multiplied(Constants.BuildingDefaultLootK);

        public void Destroy(IMilitaryObject destroyer)
        {
            World.SetBuilding(
                Position,
                CreateByPrototype(Constants.DestroyedBuilding, Player.Neutral, World, Position, Constants, this));
        }

        #endregion


        public Action<int> OnPeopleCreated { get; set; }

        public Action OnHungerChanged { get; set; }



        /// <summary>
        /// Testing ctor
        /// </summary>
        public Building() { }

        public Building(string name, IResources price)
        {
            Name = name;
            Price = price;
        }
        
        public static Building CreateByPrototype(
            Building prototype, 
            Player owner, 
            World world, 
            Vector position, 
            GameConstants constants, 
            Building previous = null)
        {
            var result = (Building) prototype.MemberwiseClone();
            
            result.Owner = owner;
            result.Prototype = prototype;
            result.World = world;
            result.Position = position;
            result.Constants = constants;

            result.SetDefaultValues(previous);

            result.Owner?.OnBuildingBegin?.Invoke(result);
            result.Owner?.OwnBuildings.Add(result);

            return result;
        }

        protected virtual void SetDefaultValues(Building previous = null)
        {
            Builders = previous?.Builders ?? 0;
            FreePeople = previous?.FreePeople ?? 0;
            Armies = previous?.Armies ?? new List<Army>();
            ContainsPeople = 0;
        }

        ~Building()
        {
            ContainsPeople = 0;
        }



        public virtual void Tick(TimeSpan deltaTime)
        {
            // armies

            foreach (var army in Armies)
            {
                army.Tick(deltaTime);
            }

            // building

            if (!Finished)
            {
                BuildingTime -=
                    Prototype.Builders != 0
                        ? deltaTime
                            .Multiple((float)Builders / Prototype.Builders)
                            .Multiple(ArePeopleHungry ? Constants.HungerK : 1)
                        : deltaTime;

                if (Finished = BuildingTime <= TimeSpan.Zero)
                {
                    ContainsPeople = Prototype.ContainsPeople;  

                    Owner?.OnBuildingEnd?.Invoke(this);
                }
            }

            if (Owner == Player.Neutral)
            {
                return;
            }

            TickAction?.Invoke(deltaTime);

            // people appearance

            if (TotalPeople != 0 && Owner.BirthrateK != 0)
            {
                PersonCreationTime += deltaTime;

                var creationPeriod = Constants.PeopleGenerationSize.Multiple(1f / TotalPeople / Owner.BirthrateK);
                var times = Math.Max(
                    0,
                    Math.Min(
                        Owner.MaxPeople - Owner.TotalPeople,
                        (int)Math.Floor(
                            (float)PersonCreationTime.Ticks
                            / creationPeriod.Ticks)));

                TotalPeople += times;
                PersonCreationTime -= creationPeriod.Multiple(times);

                OnPeopleCreated?.Invoke(times);
            }

            // people consumption

            if (Constants.PersonConsumptionPeriod != TimeSpan.Zero)
            {
                var currentConsumption
                    = Constants.PersonConsumption.Multiplied(
                        (float) deltaTime.Ticks /
                        Constants.PersonConsumptionPeriod.Ticks);

                Owner.Resources =
                    (ArePeopleHungry = !Owner.Resources.Enough(currentConsumption))
                    ? Constants.ZeroResources
                    : Owner.Resources.Substracted(currentConsumption);
            }
        }

        public virtual bool IsUpgradePossible(Building to)
            => Finished
               && Upgrades.Contains(to)
               && Owner.Resources.Enough(to.Price)
               && to.RequiredResearches.All(r => Owner.ResearchedTechnologies.Contains(r));
    }
}