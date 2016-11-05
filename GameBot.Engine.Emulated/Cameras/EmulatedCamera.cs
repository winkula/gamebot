using System;
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

        private readonly bool _addNoise;

        public EmulatedCamera(IConfig config, Emulator emulator)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (emulator == null) throw new ArgumentNullException(nameof(emulator));

            _addNoise = config.Read("Robot.Camera.Noise", false);
            _emulator = emulator;
        }

        public IImage Capture()
        {
            Image<Gray, byte> image;
            lock (_emulator)
            {
                image = new Image<Gray, byte>(_emulator.Display);
            }

            if (_addNoise)
            {
                const double noiseLevel = 0.75;
                const int gaussSize = 13;

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
