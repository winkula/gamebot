using GameBot.Core;
using GameBot.Emulation;
using System;

namespace GameBot.Engine.Emulated.Clocks
{
    public class EmulatedClock : IClock
    {
        private Emulator _emulator;

        public TimeSpan Time { get { return _emulator.Time; } }

        public EmulatedClock(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void Start()
        {
            // ignore
        }

        public void Sleep(int miliseconds)
        {
            _emulator.Execute(TimeSpan.FromMilliseconds(miliseconds));
        }
    }
}
