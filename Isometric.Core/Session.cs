using System;
using System.Threading;
using Isometric.Core.Common;
using Isometric.Core.Managers;
using Isometric.Core.Time;

namespace Isometric.Core
{
    public class Session
    {
        public World World { get; set; }

        public Building[] AllBuildingPrototypes { get; set; }

        public Research RootResearch { get; set; }


        public Clocks Clocks { get; set; }

        public PeopleManager PeopleManager { get; set; }

        public ArmiesManager ArmiesManager { get; set; }

        public VisionManager VisionManager { get; set; }


        public float GameSpeedK = 1;



        /// <summary>
        /// testing ctor
        /// </summary>
        public Session() { }

        public Session(World world, Building[] allBuildingPrototypes, Research root)
        {
            World = world;
            AllBuildingPrototypes = allBuildingPrototypes;

            PeopleManager = new PeopleManager(World);
            ArmiesManager = new ArmiesManager();
            VisionManager = new VisionManager();

            Clocks = new Clocks(World, PeopleManager, ArmiesManager);
            RootResearch = root;

            World.OnPlayerCreate += p =>
            {
                Clocks.AddTimeObject(p);
            };
        }

        public void StartClocks(TimeSpan period)
        {
            while (true)
            {
                Clocks.Tick(period.Multiple(GameSpeedK));
                Thread.Sleep(period);
            }
        }

        public Thread StartClocksThread(TimeSpan period)
        {
            var thread = new Thread(() => StartClocks(period)) {Name = "Clocks thread"};
            thread.Start();
            return thread;
        }
    }
}