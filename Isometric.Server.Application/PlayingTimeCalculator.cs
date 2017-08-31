using System;
using System.Threading;
using Isometric.Core.Common;

namespace Isometric.Server.Application
{
    public class PlayingTimeCalculator<T>
    {
        public TimeSpan PlayingTime { get; set; } = new TimeSpan();

        public TimeSpan CalculatingPeriod { get; set; } = TimeSpan.FromSeconds(1);

        public Server<T> Server { get; set; }



        public PlayingTimeCalculator(Server<T> server)
        {
            Server = server;
        } 



        public void Start()
        {
            while (true)
            {
                PlayingTime += CalculatingPeriod.Multiple(Server.Connections.Count);

                Thread.Sleep(CalculatingPeriod);
            }
        }
    }
}