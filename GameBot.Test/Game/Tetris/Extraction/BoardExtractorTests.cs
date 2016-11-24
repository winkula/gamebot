using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;
using GameBot.Core;
using GameBot.Core.Quantizers;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class BoardExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IMatcher _matcher;
        private IBoardExtractor _boardExtractor;
        private IQuantizer _quantizer;
        private Board _initialBoard;

        [TestFixtureSetUp]
        public void Init()
        {
            _matcher = new MorphologyMatcher();
            _boardExtractor = new BoardExtractor(_matcher);
            _quantizer = new MorphologyQuantizer(TestHelper.GetFakeConfig().Object);
            _initialBoard = TestHelper.BuildBoard(new[]
            {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,1,1,1,0,1,1,1,1,1,
                1,1,1,1,0,1,1,1,1,1
            });
        }

        [Test]
        public void MultiplayerUpdateNoChanges()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_0.png", _quantizer);
            
            var newBoard = _boardExtractor.MultiplayerUpdate(screenshot, new Board(_initialBoard));
            var expected = _initialBoard;

            Assert.AreEqual(expected, newBoard);
        }

        [Test]
        public void MultiplayerUpdateChangeHole0()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_1.png", _quantizer);

            var newBoard = _boardExtractor.MultiplayerUpdate(screenshot, new Board(_initialBoard));
            var expected = TestHelper.BuildBoard(new[]
            {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,1,1,1,0,1,1,1,1,1,
                1,1,1,1,0,1,1,1,1,1,
                0,1,1,1,1,1,1,1,1,1
            });

            Assert.AreEqual(expected, newBoard);
        }

        [Test]
        public void MultiplayerUpdateChangeHole6()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_2.png", _quantizer);

            var newBoard = _boardExtractor.MultiplayerUpdate(screenshot, new Board(_initialBoard));
            var expected = TestHelper.BuildBoard(new[]
            {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,1,1,1,0,1,1,1,1,1,
                1,1,1,1,0,1,1,1,1,1,
                1,1,1,1,1,1,0,1,1,1
            });

            Assert.AreEqual(expected, newBoard);
        }
    }
}
