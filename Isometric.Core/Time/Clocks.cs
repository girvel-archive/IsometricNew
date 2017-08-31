using System;
using System.Collections.Generic;

namespace Isometric.Core.Time
{
    public class Clocks
    {
        private readonly List<ITimeObject> _timeObjects;

        private readonly object _timeObjectsLock = new object();


        /// <summary>
        /// testing ctor
        /// </summary>
        public Clocks() { }

        public Clocks(params ITimeObject[] timeObjects)
        {
            _timeObjects = new List<ITimeObject>(timeObjects);
        }

        public void AddTimeObject(ITimeObject timeObject)
        {
            lock (_timeObjectsLock)
            {
                _timeObjects.Add(timeObject);
            }
        }





        public virtual void Tick(TimeSpan deltaTime)
        {
            lock (_timeObjectsLock)
            {
                foreach (var timeObject in _timeObjects)
                {
                    timeObject.Tick(deltaTime);
                }
            }
        }
    }
}