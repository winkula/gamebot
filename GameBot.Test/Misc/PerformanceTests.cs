using Emgu.CV;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class PerformanceTests
    {   
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Capture _capture;
        private Mat _image;

        [TestFixtureSetUp]
        public void Init()
        {
            _capture = new Capture(0);
        }

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
                //QueryFrame();

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

        private void Grab(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                _capture.Grab();
                _capture.Retrieve(_image, 0);
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
