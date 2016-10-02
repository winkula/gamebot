using Emgu.CV;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Test.Performance
{
    [TestFixture]
    public class PerformanceTests
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private Capture capture = new Capture(0);
        private IImage image;

        [Test]
        public void PerformanceVideoCapture()
        {
            int num = 10;
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
            int num = 10;
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
