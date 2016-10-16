﻿using GameBot.Core;
using GameBot.Emulation;
using Emgu.CV;
using Emgu.CV.Structure;

namespace GameBot.Engine.Emulated.Cameras
{
    public class EmulatedCamera : ICamera
    {
        private readonly Emulator _emulator;

        public int Width => GameBoyConstants.ScreenWidth;
        public int Height => GameBoyConstants.ScreenHeight;

        public EmulatedCamera(Emulator emulator)
        {
            _emulator = emulator;
        }

        public IImage Capture()
        {
            return new Image<Gray, byte>(_emulator.Display);
        }
    }
}
