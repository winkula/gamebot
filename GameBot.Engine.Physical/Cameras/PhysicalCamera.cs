using System;
using System.IO;
using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Physical.Cameras
{
    public class PhysicalCamera : ICamera
    {
        private readonly Capture _capture;

        public int Width => _capture.Width;
        public int Height => _capture.Height;

        public PhysicalCamera(IConfig config)
        {
            var cameraIndex = config.Read("Robot.Camera.Index", 0);

            _capture = new Capture(cameraIndex);
            _capture.Start();
        }

        public IImage Capture()
        {
            var src = new Mat();
            _capture.Grab();
            _capture.Retrieve(src);

            // save every 30th image
            var tick = DateTime.Now.Ticks;
            if (tick % 30 == 0)
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Images"));
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Images", $"image{tick}.png");
                src.Save(path);
            }

            return src;
        }
    }
}
