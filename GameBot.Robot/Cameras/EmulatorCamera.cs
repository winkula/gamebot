using GameBot.Core;
using GameBot.Emulation;
using System.Drawing;

namespace GameBot.Robot.Cameras
{
    public class EmulatorCamera : ICamera
    {
        private readonly Emulator emulator;

        public EmulatorCamera(Emulator emulator)
        {
            this.emulator = emulator;
        }

        public Image Capture()
        {
            return emulator.Display;
        }
    }
}
