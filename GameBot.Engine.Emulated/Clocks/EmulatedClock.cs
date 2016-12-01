using GameBot.Core;
using GameBot.Emulation;
using System;

namespace GameBot.Engine.Emulated.Clocks
{
    public class EmulatedClock : IClock
    {
        private readonly Emulator _emulator;

        public TimeSpan Time
        {
            get
            {
                lock (_emulator)
                {
                    return _emulator.Time;
                }
            }
        }

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
            if (miliseconds < 0) throw new ArgumentException("miliseconds can't be negative.");

            var timeSpan = TimeSpan.FromMilliseconds(miliseconds);
            var frames = _emulator.GetExecutionDurationInFrames(timeSpan);
            SleepInternal(frames);
        }

        public void Sleep(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero) throw new ArgumentException("timeSpan can't be negative.");

            var frames = _emulator.GetExecutionDurationInFrames(timeSpan);
            SleepInternal(frames);
        }

        private void SleepInternal(int frames)
        {
            const int frameStep = 3;
            for (int i = 0; i < frames; i += frameStep)
            {
                lock (_emulator)
                {
                    _emulator.Execute(frameStep);
                }
            }
        }
    }
}
