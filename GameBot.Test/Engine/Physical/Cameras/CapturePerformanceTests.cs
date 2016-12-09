using System.Diagnostics;
using Emgu.CV;
using NUnit.Framework;

namespace GameBot.Test.Engine.Physical.Cameras
{
    [TestFixture]
    public class CapturePerformanceTests
    {   
        private Capture _capture;

        [TestFixtureSetUp]
        public void Init()
        {
            _capture = new Capture(0);
        }

        [Ignore]
        [Test]
        public void MeasureFramerate()
        {
            var sw = new Stopwatch();

            int num = 30;

            var image = new Mat();
            
            _capture.Start();
            sw.Start();

            for (int i = 0; i < num; i++)
            {
                _capture.Grab();
                _capture.Retrieve(image);
            }

            _capture.Stop();
            sw.Stop();

            Debug.WriteLine($"Resolution: {image.Width} x {image.Height}");
            Debug.WriteLine($"Time for {num} loops: {sw.ElapsedMilliseconds} ms");
            Debug.WriteLine($"Estimated fps: {num / (sw.ElapsedMilliseconds / 1000.0)}");

            _capture.Stop();
        }
    }
}
