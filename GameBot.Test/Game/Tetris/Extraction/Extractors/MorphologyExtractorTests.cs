using GameBot.Core;
using GameBot.Core.Quantizers;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Extractors;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction.Extractors
{
    [TestFixture]
    public class MorphologyExtractorTests
    {
        private IExtractor _extractor;
        private IQuantizer _quantizer;

        [TestFixtureSetUp]
        public void Init()
        {
            var config = TestHelper.GetFakeConfig().Object;
            _extractor = new MorphologyExtractor(config);
            _quantizer = new MorphologyQuantizer(config);
        }

        [Test]
        public void ExtractCurrentPiecePerformance()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);
            var tetrimino = Tetrimino.Z;
            var searchHeight = 8;

            for (int i = 0; i < 100; i++)
            {
                var piece = _extractor.ExtractCurrentPiece(screenshot, tetrimino, searchHeight);
            }
        }

        [Test]
        public void ExtractNextPiecePerformance()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_play_1.png", _quantizer);
            
            for (int i = 0; i < 100; i++)
            {
                var tetrimino = _extractor.ExtractNextPiece(screenshot);
            }
        }
    }
}
