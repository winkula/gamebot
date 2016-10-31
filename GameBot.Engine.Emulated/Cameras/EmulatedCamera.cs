using GameBot.Core;
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
            // TODO: move to config file
            const bool addNoise = true;
            const double noiseLevel = 0.75;
            const int gaussSize = 13;

            var image = new Image<Gray, byte>(_emulator.Display);

            if (addNoise)
            {
                var noise = new Image<Gray, byte>(image.Size);
                noise.SetRandNormal(new MCvScalar(0), new MCvScalar(255));
                noise = noise.SmoothGaussian(gaussSize);
                
                image = image.AddWeighted(noise, 1 - noiseLevel, noiseLevel, 0);
                image = image.Mul(0.5).Add(new Gray(100));
            }

            return image;
        }
    }
}
