using System;
using System.Collections.Generic;
using System.Linq;
using Isometric.Core.Buildings;
using Isometric.Core.Time;

namespace Isometric.Core.Managers
{
    public class PeopleManager : ITimeObject
    {
        public World World { get; set; }

        public List<Player> Players = new List<Player>();

        public List<Func<bool>> PeopleManagementActions { get; set; } = new List<Func<bool>>();



        /// <summary>
        /// testing ctor
        /// </summary>
        public PeopleManager() { }

        public PeopleManager(World world)
        {
            World = world;

            World.OnPlayerCreate += p =>
            {
                p.OnBuildingBegin += b =>
                {
                    b.OnPeopleCreated += n =>
                    {
                        p.Area[0, 0].FreePeople += n;
                        b.FreePeople -= n;
                    };
                    
                    PeopleManagementActions.Add(
                        () => TryAddBuilders(
                            b,
                            Math.Min(
                                b.Prototype.Builders - b.Builders,
                                b.Owner.Area[0, 0].FreePeople)));
                    Tick(TimeSpan.Zero);
                };

                p.OnBuildingEnd += b =>
                {
                    var incomeBuilding = b as WorkerBuilding;

                    if (incomeBuilding != null)
                    {
                        PeopleManagementActions.Add(
                            () => TryAddBuilders(
                                incomeBuilding,
                                -incomeBuilding.Builders));

                        PeopleManagementActions.Add(
                            () => TryAddWorkers(
                                incomeBuilding,
                                Math.Min(
                                    incomeBuilding.Prototype.Workers - incomeBuilding.Workers,
                                    incomeBuilding.Owner.Area[0, 0].FreePeople)));
                        Tick(TimeSpan.Zero);
                    }
                };
            };
        }



        public void Tick(TimeSpan deltaTime)
        {
            for (var i = 0; i < PeopleManagementActions.Count; )
            {
                var action = PeopleManagementActions[i];
                lock (action)
                if (action())
                {
                    PeopleManagementActions.Remove(action);
                }
                else
                {
                    i++;
                }
            }
        }

        public bool TryAddWorkers(WorkerBuilding building, int deltaWorkers)
        {
            var resultWorkers = building.Workers + deltaWorkers;
            var resultFreePeople = building.Owner.Area[0, 0].FreePeople - deltaWorkers;

            if (!building.Finished
                || resultWorkers < 0
                || resultWorkers > building.Prototype.Workers
                || resultFreePeople < 0) return false;

            building.Workers = resultWorkers;
            building.Owner.Area[0, 0].FreePeople = resultFreePeople;

            return true;
        }

        public bool TryAddBuilders(Building building, int deltaBuilders)
        {
            var resultBuilders = building.Builders + deltaBuilders;
            var resultFreePeople = building.Owner.Area[0, 0].FreePeople - deltaBuilders;

            if (resultBuilders < 0
                || resultBuilders > building.Prototype.Builders
                || resultFreePeople < 0) return false;

            building.Builders = resultBuilders;
            building.Owner.Area[0, 0].FreePeople = resultFreePeople;

            return true;
        }

        public bool TryMovePeople(Building to, int movingPeople, Player peopleOwner = null)
        {
            var player = peopleOwner ?? to.Owner;

            if (player.Area[0, 0].FreePeople < movingPeople)
            {
                return false;
            }

            player.Area[0, 0].FreePeople -= movingPeople;
            to.FreePeople += movingPeople;

            return true;
        }

        public int AddWorkersForPlayer(WorkerBuilding prototype, int deltaWorkers, Player player)
        {
            var oldDeltaWorkers = deltaWorkers;

            foreach (var building in player.OwnBuildings.Where(b => b.Prototype == prototype).OfType<WorkerBuilding>())
            {
                var delta = deltaWorkers > 0    
                    ? Math.Min(building.Prototype.Workers - building.Workers, deltaWorkers) 
                    : Math.Max(-building.Workers, deltaWorkers);

                if (TryAddWorkers(building, delta))
                {
                    deltaWorkers -= delta;
                }
            }

            return oldDeltaWorkers - deltaWorkers;
        }
    }
}