using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;

namespace GameBot.Robot.Cameras
{
    public class Camera : ICamera
    {
        private readonly IConfig config;
        private readonly Capture capture;

        public int Width { get { return capture.Width; } }
        public int Height { get { return capture.Height; } }

        public Camera(IConfig config)
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
