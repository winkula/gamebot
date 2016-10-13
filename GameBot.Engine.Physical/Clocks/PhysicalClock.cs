using GameBot.Core;
using System;
using System.Threading;

namespace GameBot.Engine.Physical.Clocks
{
    public class PhysicalClock : IClock
    {
        private DateTime StartTime { get; set; }
        public TimeSpan Time { get { return DateTime.Now - StartTime; } }

        public void Start()
        {
            StartTime = DateTime.Now;
        }

        public void Sleep(int miliseconds)
        {
            Thread.Sleep(miliseconds);
        }
    }
}
