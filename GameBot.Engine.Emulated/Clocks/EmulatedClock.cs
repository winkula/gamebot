using GameBot.Core;
using GameBot.Emulation;
using System;

namespace GameBot.Engine.Emulated.Clocks
{
    public class EmulatedClock : IClock
    {
        private Emulator emulator;

        public TimeSpan Time { get { return emulator.Time; } }

        public EmulatedClock(Emulator emulator)
        {
            this.emulator = emulator;
        }

        public void Start()
        {
            // ignore
        }

        public void Sleep(int miliseconds)
        {
            emulator.Execute(TimeSpan.FromMilliseconds(miliseconds));
        }
    }
}
