using GameBot.Core;
using GameBot.Core.Quantizers;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction.Matchers
{
    public class MorphologyMatcherTests
    {
        private IMatcher _matcher;
        private IQuantizer _quantizer;

        [SetUp]
        public void Init()
        {
            var config = TestHelper.GetFakeConfig().Object;

            _matcher = new MorphologyMatcher();
            _quantizer = new MorphologyQuantizer(config);
        }

        [TestCase(0, 0, 0.0)]
        [TestCase(9, 0, 0.0)]
        [TestCase(9, 17, 0.0)]
        [TestCase(0, 17, 0.0)]
        [TestCase(2, 10, 0.0)]
        [TestCase(3, 9, 0.0)]
        [TestCase(4, 8, 0.0)]
        [TestCase(5, 8, 0.0)]
        [TestCase(6, 9, 0.0)]
        [TestCase(5, 10, 0.0)]
        [TestCase(3, 11, 0.0)]
        [TestCase(4, 11, 0.0)]
        [TestCase(3, 10, 1.0)]
        [TestCase(4, 10, 1.0)]
        [TestCase(4, 9, 1.0)]
        [TestCase(5, 9, 1.0)]
        public void TileProbability(int x, int y, double expected)
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_2.png", _quantizer);

            var probability = _matcher.GetProbabilityBoardBlock(screenshot, x, y);

            Assert.AreEqual(expected, probability);
        }

        [Test]
        public void CurrentPieceProbabilityZero()
        {
            var piece = new Piece(Tetrimino.S);
            var screenshot = TestHelper.GetScreenshot("Screenshots/white.png", _quantizer);

            var probability = _matcher.GetProbabilityCurrentPiece(screenshot, piece);

            Assert.AreEqual(0.0, probability);
        }

        [Test]
        public void CurrentPieceProbabilityOne()
        {
            var piece = new Piece(Tetrimino.S);
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);

            var probability = _matcher.GetProbabilityCurrentPiece(screenshot, piece);

            Assert.AreEqual(1.0, probability);
        }

        [Test]
        public void CurrentPieceProbabilityLessThanOne()
        {
            var piece = new Piece(Tetrimino.Z);
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);

            var probability = _matcher.GetProbabilityCurrentPiece(screenshot, piece);

            Assert.Greater(probability, 0.0);
            Assert.Less(probability, 1.0);
        }

        [Test]
        public void NextPieceProbabilityOne()
        {
            var tetrimino = Tetrimino.L;
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);

            var probability = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);

            Assert.AreEqual(1.0, probability);
        }

        [Test]
        public void NextPieceProbabilityLessThanOne()
        {
            var tetrimino = Tetrimino.O;
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);

            var probability = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);

            Assert.Greater(probability, 0.0);
            Assert.Less(probability, 1.0);
        }

        [Test]
        public void NextPieceProbabilityZero()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/white.png", _quantizer);

            foreach (var tetrimino in Tetriminos.All)
            {
                var probability = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);
                Assert.AreEqual(0.0, probability);
            }
        }

        [Test]
        public void MatchMany()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);
            
            for (int i = 0; i < 100; i++)
            {
                for (int x = 0; x < TetrisConstants.DefaultBoardWidth; x++)
                {
                    for (int y = 0; y < TetrisConstants.DefaultBoardHeight; y++)
                    {
                        _matcher.GetProbabilityBoardBlock(screenshot, x, y);
                    }
                }
            }
        }
    }
}
