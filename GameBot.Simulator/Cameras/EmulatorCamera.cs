using GameBot.Core;
using GameBot.Emulation;
using Emgu.CV;
using Emgu.CV.Structure;

namespace GameBot.Robot.Cameras
{
    public class EmulatorCamera : ICamera
    {
        private readonly Emulator emulator;

        public int Width { get { return GameBoyConstants.ScreenWidth; } }
        public int Height { get { return GameBoyConstants.ScreenHeight; } }

        public EmulatorCamera(Emulator emulator)
        {
            this.emulator = emulator;
        }

        public IImage Capture()
        {
            return new Image<Gray, byte>(emulator.Display);
        }
    }
}
