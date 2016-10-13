using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Physical.Cameras
{
    public class PhysicalCamera : ICamera
    {
        private readonly IConfig config;
        private readonly Capture capture;

        public int Width { get { return capture.Width; } }
        public int Height { get { return capture.Height; } }

        public PhysicalCamera(IConfig config)
        {
            this.config = config;

            this.capture = new Capture(config.Read("Robot.Camera.Index", 0));
            this.capture.Start();
        }

        public IImage Capture()
        {
            var src = new Mat();
            var dst = new Mat();
            capture.Grab();
            capture.Retrieve(src, 0);
            //CvInvoke.CvtColor(src, dst, ColorConversion.Rgb2Gray);

            // var dst = new Mat();
            // var captured =  capture.QueryFrame();

            return src;
        }
    }
}
