using Emgu.CV;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class PerformanceTests
    {   
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Capture _capture;
        private IImage _image;

        [TestFixtureSetUp]
        public void Init()
        {
            _capture = new Capture(0);
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

        private void Grab()
        {
            _capture.Grab();
            _capture.Retrieve(_image, 0);
        }

        private void QueryFrame()
        {
            _image = _capture.QueryFrame();
        }
    }
}
