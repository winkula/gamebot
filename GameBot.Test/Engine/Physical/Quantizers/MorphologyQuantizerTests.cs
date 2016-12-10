using System.Linq;
using GameBot.Core.Quantizers;
using NUnit.Framework;

namespace GameBot.Test.Engine.Physical.Quantizers
{
    [TestFixture]
    public class MorphologyQuantizerTests
    {
        [Test]
        public void TastMany()
        {
            var testData = TestDataFactory.Data.First(x => x.ImageKey == "0500");
            
            var configMock = TestHelper.GetFakeConfig();
            var quantizer = new MorphologyQuantizer(configMock.Object);
            quantizer.Keypoints = testData.Keypoints;

            for (int i = 0; i < 100; i++)
            {
                quantizer.Quantize(testData.Image);
            }
        }
    }
}
