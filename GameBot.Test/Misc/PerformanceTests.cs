using System;
using Emgu.CV;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;
using Emgu.CV.CvEnum;

namespace GameBot.Test.Misc
{
    [TestFixture]
    public class PerformanceTests
    {   
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Capture _capture;
        private Mat _image;
        private int numImagesGrabbed = 0;

        [TestFixtureSetUp]
        public void Init()
        {
            _capture = new Capture(1);
            _capture.ImageGrabbed += (sender, args) =>
            {
                numImagesGrabbed++;
                _capture.Retrieve(_image);
                //_capture.SetCaptureProperty(CapProp.Fps, 60.0);
            };

            foreach (var value in Enum.GetValues(typeof(CapProp)))
            {
                Debug.Write($"Cam {value}:       {_capture.GetCaptureProperty((CapProp) value)}");
            }
            
            _capture.SetCaptureProperty(CapProp.Focus, 100);
            _capture.SetCaptureProperty(CapProp.Brightness, 1.0);
            _capture.SetCaptureProperty(CapProp.FrameWidth, 320);
            _capture.SetCaptureProperty(CapProp.FrameHeight, 240);
            _capture.SetCaptureProperty(CapProp.Fps, 60.0);
        }

        [Ignore]
        [Test]
        public void MeasureFramerate()
        {
            int num = 30;

            _image = new Mat();

            /*
            for (int i = 0; i < 10; i++)
            {
                //_capture.SetCaptureProperty(CapProp.Focus, i * 10);
                Grab(1, true);
            }
            */

            numImagesGrabbed = 0;
            _capture.Start();
            _stopwatch.Restart();

            Thread.Sleep(1000);
            //Grab(num);

            _capture.Stop();
            _stopwatch.Stop();

            Debug.Write($"Resolution: {_image.Width} x {_image.Height}");
            Debug.Write($"Time for {num} loops: {_stopwatch.ElapsedMilliseconds} ms");
            Debug.Write($"Estimated fps: {num / (_stopwatch.ElapsedMilliseconds / 1000.0)}");
            Debug.Write($"Num images grabbed: {numImagesGrabbed}");

            _capture.Stop();
        }

        [Ignore]
        [Test]
        public void DurationForAFrame()
        {
            var watch = new Stopwatch();
            int num = 30;
            
            _capture.Start();
            
            for (int i = 0; i < num; i++)
            {
                //Thread.Sleep(100);

                watch.Start();
                _image = new Mat();

                //QueryFrame(2);
                Grab(2);

                watch.Stop();

                CvInvoke.Imshow("Test", _image);
                CvInvoke.WaitKey();
            }
            
            Debug.Write($"Time for {num} loops: {watch.ElapsedMilliseconds} ms");

            _capture.Stop();
        }

        [Ignore]
        [Test]
        public void DurationForAFrameAlwaysNewCapture()
        {
            var watch = new Stopwatch();
            int num = 30;
            
            for (int i = 0; i < num; i++)
            {
                Thread.Sleep(100);

                watch.Start();

                using (var capture = new Capture(0))
                {
                    //capture.Start();
                    _image = new Mat();
                    _image = capture.QueryFrame();
                    //_capture.Stop();
                }

                watch.Stop();

                CvInvoke.Imshow("Test", _image);
                CvInvoke.WaitKey();
            }

            Debug.Write($"Time for {num} loops: {watch.ElapsedMilliseconds} ms");

        }

        [Ignore]
        [Test]
        public void PerformanceVideoCapture()
        {
            int num = 30;
            bool show = false;

            _capture.Start();
            
            _stopwatch.Restart();

            _image = new Mat();
            for (int i = 0; i < num; i++)
            {
                Grab();
            }

            _stopwatch.Stop();
            Debug.Write($"Time for {num} loops: {_stopwatch.ElapsedMilliseconds} ms");

            _capture.Stop();
        }

        [Ignore]
        [Test]
        public void PerformanceQueryFrame()
        {
            int num = 30;
            bool show = false;
            
            _stopwatch.Restart();

            _image = new Mat();
            for (int i = 0; i < num; i++)
            {
                //Grab();
                QueryFrame();

                if (show)
                {
                    CvInvoke.Imshow("Test", _image);
                    CvInvoke.WaitKey();
                }
            }

            _stopwatch.Stop();
            Debug.Write($"Time for {num} loops: {_stopwatch.ElapsedMilliseconds} ms");

            _capture.Stop();
        }

        private void Grab(int num = 1, bool show = false)
        {
            for (int i = 0; i < num; i++)
            {
                _capture.Grab();
                _capture.Retrieve(_image, 0);

                if (show)
                {
                    CvInvoke.Imshow("Test", _image);
                    CvInvoke.WaitKey();
                }
            }
        }

        private void QueryFrame(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                _image = _capture.QueryFrame();
            }
        }
    }
}
