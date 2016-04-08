using GameBot.Core;
using System;

namespace GameBot.Robot
{
    public class TimeProvider : ITimeProvider
    {
        private DateTime StartTime { get; set; }
        public TimeSpan Time { get { return DateTime.Now - StartTime; } }

        public void Start()
        {
            StartTime = DateTime.Now;
        }
    }
}
