using GameBot.Core;
using GameBot.Emulation;
using System;

namespace GameBot.Engine.Emulated.Clocks
{
    public class EmulatedClock : IClock
    {
        private readonly Emulator _emulator;

        public TimeSpan Time => _emulator.Time;

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
