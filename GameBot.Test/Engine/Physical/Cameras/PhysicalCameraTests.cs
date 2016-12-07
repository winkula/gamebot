using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using GameBot.Core;
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

        [TestFixtureSetUp]
        public void Init()
        {
            _configMock = TestHelper.GetFakeConfig();
            _camera = new PhysicalCamera(_configMock.Object);
        }

        [Test]
        public void TestMultipleWithProcessing()
        {
            Mat last = null;
            var delay = 20;
            for (int i = 0; i < 30; i++)
            {
                var image = _camera.Capture(last);
                Thread.Sleep(delay);

                last = image;
            }
        }
    }
}
