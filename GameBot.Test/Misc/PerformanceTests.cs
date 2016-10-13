using Emgu.CV;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class PerformanceTests
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private Capture capture;
        private IImage image;

        [TestFixtureSetUp]
        public void Init()
        {
            capture = new Capture(0);
        }

        [Test]
        public void PerformanceVideoCapture()
        {
            int num = 30;
            bool show = false;

            capture.Start();
            
            stopwatch.Restart();

            image = new Mat();
            for (int i = 0; i < num; i++)
            {
                Grab();
                //QueryFrame();

                if (show)
                {
                    CvInvoke.Imshow("Test", image);
                    CvInvoke.WaitKey();
                }
            }

            stopwatch.Stop();
            Debug.Write($"Time for {num} loops: {stopwatch.ElapsedMilliseconds} ms");

            capture.Stop();
        }

        [Test]
        public void PerformanceQueryFrame()
        {
            int num = 30;
            bool show = false;
            
            stopwatch.Restart();

            image = new Mat();
            for (int i = 0; i < num; i++)
            {
                //Grab();
                QueryFrame();

                if (show)
                {
                    CvInvoke.Imshow("Test", image);
                    CvInvoke.WaitKey();
                }
            }

            stopwatch.Stop();
            Debug.Write($"Time for {num} loops: {stopwatch.ElapsedMilliseconds} ms");

            capture.Stop();
        }

        private void Grab()
        {
            capture.Grab();
            capture.Retrieve(image, 0);
        }

        private void QueryFrame()
        {
            image = capture.QueryFrame();
        }
    }
}
