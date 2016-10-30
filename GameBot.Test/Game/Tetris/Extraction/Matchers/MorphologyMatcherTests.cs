using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction.Matchers
{
    [TestFixture]
    public class MorphologyMatcherTests
    {
        private IMatcher _matcher;
        
        [SetUp]
        public void Init()
        {
            _matcher = new MorphologyMatcher();
        }
        
        [Test]
        public void NextPieceProbabilityOne()
        {
            var tetrimino = Tetrimino.L;
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", DateTime.Now.Subtract(DateTime.MinValue));

            var probability = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);

            Assert.AreEqual(1.0, probability);
        }

        [Test]
        public void NextPieceProbabilityNotOne()
        {
            var tetrimino = Tetrimino.O;
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", DateTime.Now.Subtract(DateTime.MinValue));
            
            var probability = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);

            Assert.GreaterOrEqual(probability, 0.0);
            Assert.Less(probability, 1.0);
        }

        [Test]
        public void NextPieceProbabilityZero()
        {
            var image = new Mat("Screenshots/white.png", LoadImageType.Grayscale);
            var screenshot = new EmguScreenshot(image, DateTime.Now.Subtract(DateTime.MinValue));

            foreach (var tetrimino in Tetriminos.All)
            {
                var probability = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);
                Assert.AreEqual(0.0, probability);
            }
        }
    }
}
