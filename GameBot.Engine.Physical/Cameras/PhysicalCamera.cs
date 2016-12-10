using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
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
            var frameWidth = config.Read("Robot.Camera.FrameWidth", 640.0);
            var frameHeight = config.Read("Robot.Camera.FrameHeight", 480.0);
            var fps = config.Read("Robot.Camera.Fps", 30.0);

            _capture = new Capture(cameraIndex);
            _capture.SetCaptureProperty(CapProp.FrameWidth, frameWidth);
            _capture.SetCaptureProperty(CapProp.FrameHeight, frameHeight);
            _capture.SetCaptureProperty(CapProp.Fps, fps);
            if (rotateImage)
            {
                _capture.FlipHorizontal = true;
                _capture.FlipVertical = true;
            }
            _capture.Start();

            var thread = new Thread(GrabInternal) { IsBackground = true };
            thread.Start();
        }

        private void GrabInternal()
        {
            try
            {
                while (true)
                {
                    var src = new Mat();

                    _capture.Grab();
                    _capture.Retrieve(src);

                    lock (_lock)
                    {
                        _frame = src;
                        // inform threads who are waiting for a new frame
                        Monitor.PulseAll(_lock);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // ignore
            }
            finally
            {
                _capture.Stop();
                _capture.Dispose();
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
                if (predecessor == null || predecessor == _frame)
                {
                    // current frame already read. wait for new frame
                    Monitor.Wait(_lock);
                }
                frame = _frame;
            }
            return frame;
        }
    }
}
