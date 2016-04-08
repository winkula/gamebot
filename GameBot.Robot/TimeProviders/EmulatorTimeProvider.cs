using GameBot.Core;
using GameBot.Emulation;
using System;

namespace GameBot.Robot
{
    public class EmulatorTimeProvider : ITimeProvider
    {
        private Emulator emulator;
        public TimeSpan Time { get { return emulator.Time; } }

        public EmulatorTimeProvider(Emulator emulator)
        {
            this.emulator = emulator;
        }

        public void Start()
        {
            // ignore
        }
    }
}
