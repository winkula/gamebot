using System.Threading;
using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Physical.Cameras
{
    public class PhysicalCamera : ICamera
    {
        private readonly Capture _capture;

        private readonly object _lock = new object();
        private Mat _frame;

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

            var thread = new Thread(GrabInternal);
            thread.Start();
        }

        private void GrabInternal()
        {
            while (true)
            {
                var src = new Mat();

                _capture.Grab();
                _capture.Retrieve(src);

                lock (_lock)
                {
                    _frame = src;
                    Monitor.PulseAll(_lock);
                }
            }
        }

        public Mat Capture()
        {
            Mat frame;
            lock (_lock)
            {
                frame = _frame;
            }
            return frame;
        }

        public Mat Capture(Mat predecessor)
        {
            Mat frame;
            lock (_lock)
            {
                Monitor.Wait(_lock);
                frame = _frame;
            }
            return frame;
        }
    }
}
