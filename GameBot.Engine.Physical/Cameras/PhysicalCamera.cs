using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Physical.Cameras
{
    public class PhysicalCamera : ICamera
    {
        private readonly IConfig _config;
        private readonly Capture _capture;

        public int Width { get { return _capture.Width; } }
        public int Height { get { return _capture.Height; } }

        public PhysicalCamera(IConfig config)
        {
            _config = config;

            _capture = new Capture(config.Read("Robot.Camera.Index", 0));
            _capture.Start();
        }

        public IImage Capture()
        {
            var src = new Mat();
            var dst = new Mat();
            _capture.Grab();
            _capture.Retrieve(src, 0);
            //CvInvoke.CvtColor(src, dst, ColorConversion.Rgb2Gray);

            // var dst = new Mat();
            // var captured =  capture.QueryFrame();

            return src;
        }
    }
}
