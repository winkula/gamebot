using System;
using System.Drawing;
using GameBot.Core;
using GameBot.Emulation;
using Emgu.CV;
using Emgu.CV.CvEnum;
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
            Mat image = new Mat();

            lock (_emulator)
            {
                new Image<Gray, byte>(_emulator.Display).Mat.CopyTo(image);
            }

            if (_addNoise)
            {
                AddNoise(image);
            }

            return image;
        }

        private void AddNoise(Mat image)
        {
            const double noiseLevel = 0.75;
            var mean = new MCvScalar(0);
            var std = new MCvScalar(255);
            const int gaussSize = 13;
            const double scale = 0.5;
            const double shift = 100;

            var noise = new Mat(image.Size, DepthType.Cv8U, 1);

            using (ScalarArray scalarArray1 = new ScalarArray(mean))
            using (ScalarArray scalarArray2 = new ScalarArray(std))
            {
                CvInvoke.Randn(noise, scalarArray1, scalarArray2);
            }
            CvInvoke.GaussianBlur(noise, noise, new Size(gaussSize, gaussSize), 0.0);
            CvInvoke.AddWeighted(image, 1 - noiseLevel, noise, noiseLevel, 0, image, image.Depth);
            CvInvoke.ConvertScaleAbs(image, image, scale, shift);
        }
    }
}
