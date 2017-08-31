namespace Isometric.Core.Buildings
{
    public class WorkerBuilding : Building
    {
        private int _workers;

        public int Workers
        {
            get { return _workers; }
            set
            {
                var oldValue = _workers;
                _workers = value;

                TotalPeople += value - oldValue;
            }
        }

        public float Efficiency
            => (ArePeopleHungry ? Constants.HungerK : 1)
               * (Prototype.Workers == 0
                   ? 1
                   : (float) Workers / Prototype.Workers);

        public new WorkerBuilding Prototype
        {
            get { return (WorkerBuilding) base.Prototype; }
            set { base.Prototype = value; }
        }


        /// <summary>
        /// testing ctor
        /// </summary>
        public WorkerBuilding() { }

        public WorkerBuilding(string name, IResources price, int workers) : base(name, price)
        {
            Workers = workers;
        }

        protected override void SetDefaultValues(Building previous = null)
        {
            base.SetDefaultValues(previous);

            Workers = 0;
        }
    }
}