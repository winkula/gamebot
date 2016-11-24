using GameBot.Core;
using System;
using System.Threading;

namespace GameBot.Engine.Physical.Clocks
{
    public class PhysicalClock : IClock
    {
        private DateTime StartTime { get; set; }
        public TimeSpan Time => DateTime.Now - StartTime;

        public void Start()
        {
            StartTime = DateTime.Now;
        }

        public void Sleep(int miliseconds)
        {
            if (miliseconds < 0) throw new ArgumentException("miliseconds can't be negative.");

            Thread.Sleep(miliseconds);
        }

        public void Sleep(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero) throw new ArgumentException("timeSpan can't be negative.");

            Thread.Sleep((int)timeSpan.TotalMilliseconds);
        }
    }
}
