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
            var rotateImage = config.Read("Robot.Camera.RotateImage", false);

            _capture = new Capture(cameraIndex);
            if (rotateImage)
            {
                _capture.FlipHorizontal = true;
                _capture.FlipVertical = true;
            }
            _capture.Start();
        }

        public Mat Capture()
        {
            var src = new Mat();

            _capture.Grab();
            _capture.Retrieve(src);
            
            return src;
        }
    }
}
