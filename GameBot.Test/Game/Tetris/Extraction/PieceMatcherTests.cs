using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class PieceMatcherTests
    {
        private PieceMatcher _pieceMatcher;

        [SetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
        }
        
        [Test]
        public void NextPieceProbabilityOne()
        {
            var tetrimino = Tetrimino.L;
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            var probability = _pieceMatcher.GetProbabilityNextPiece(screenshot, tetrimino);

            Assert.AreEqual(1.0, probability);
        }

        [Test]
        public void NextPieceProbabilityNotOne()
        {
            var tetrimino = Tetrimino.O;
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);
            
            var probability = _pieceMatcher.GetProbabilityNextPiece(screenshot, tetrimino);

            Assert.GreaterOrEqual(probability, 0.0);
            Assert.Less(probability, 1.0);
        }

        [Test]
        public void NextPieceProbabilityZero()
        {
            var image = new Mat("Screenshots/white.png", LoadImageType.Grayscale);
            var screenshot = new EmguScreenshot(image, TimeSpan.Zero);

            foreach (var tetrimino in Tetriminos.All)
            {
                var probability = _pieceMatcher.GetProbabilityNextPiece(screenshot, tetrimino);
                Assert.AreEqual(0.0, probability);
            }
        }

        [Test]
        public void GetProbabilityPerformance()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            // 4 x 10 x 17
            for (int orientation = 0; orientation < 4; orientation++)
            {
                for (int x = -4; x <= 5; x++)
                {
                    for (int y = -16; y <= 0; y++)
                    {
                        var piece = new Piece(Tetrimino.T, orientation, x, y);

                        var probability = _pieceMatcher.GetProbability(screenshot, piece);

                        Assert.GreaterOrEqual(probability, 0.0);
                        Assert.LessOrEqual(probability, 1.0);
                    }
                }
            }
        }
    }
}
