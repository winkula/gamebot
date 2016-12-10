using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Extensions;
using GameBot.Engine.Physical.Cameras;
using Moq;
using NUnit.Framework;

namespace GameBot.Test.Engine.Physical.Cameras
{
    [TestFixture]
    public class PhysicalCameraTests
    {
        private Mock<IConfig> _configMock;
        private ICamera _camera;
        private Capture _capture;

        [TestFixtureSetUp]
        public void Init()
        {
            _configMock = TestHelper.GetFakeConfig();
            _camera = new PhysicalCamera(_configMock.Object);
            _capture = new Capture(0);
        }

        [Test]
        public void ReadProperties()
        {
            var properties = _capture.ReadProperties();
            Debug.WriteLine(properties);
        }

        [Test]
        public void CaptureAlwaysNew()
        {
            Mat last = null;
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 30; i++)
            {
                var image = _camera.Capture(last);
                Thread.Sleep(100);

                last = image;
            }

            sw.Stop();
            Debug.WriteLine($"{sw.ElapsedMilliseconds} ms elapsed");
        }

        [Test]
        public void CaptureFast()
        {
            for (int i = 0; i < 30; i++)
            {
                var image = _camera.Capture();
            }
        }
    }
}
