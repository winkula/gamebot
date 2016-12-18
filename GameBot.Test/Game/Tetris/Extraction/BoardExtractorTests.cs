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

            var board = new Board(_initialBoard);
            var lines = _boardExtractor.MultiplayerRaisedLines(screenshot, board);
            var position = _boardExtractor.MultiplayerHolePosition(screenshot, board);

            var newBoard = _boardExtractor.MultiplayerAddLines(board, lines, position.Result);
            var expected = _initialBoard;

            Assert.AreEqual(0, lines);
            Assert.AreEqual(expected, newBoard);
        }

        [Test]
        public void MultiplayerUpdateChangeHole0()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_1.png", _quantizer);

            var board = new Board(_initialBoard);
            var lines = _boardExtractor.MultiplayerRaisedLines(screenshot, board);
            var position = _boardExtractor.MultiplayerHolePosition(screenshot, board);

            var newBoard = _boardExtractor.MultiplayerAddLines(board, lines, position.Result);
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

            Assert.AreEqual(1, lines);
            Assert.AreEqual(0, position.Result);
            Assert.AreEqual(expected, newBoard);
        }

        [Test]
        public void MultiplayerUpdateChangeHole6()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_2.png", _quantizer);
            var board = new Board(_initialBoard);

            var lines = _boardExtractor.MultiplayerRaisedLines(screenshot, board);
            var position = _boardExtractor.MultiplayerHolePosition(screenshot, board);

            var newBoard = _boardExtractor.MultiplayerAddLines(board, lines, position.Result);
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

            Assert.AreEqual(1, lines);
            Assert.AreEqual(6, position.Result);
            Assert.AreEqual(expected, newBoard);
        }

        [Test]
        public void MultiplayerUpdateChangeHole8MultipleLines()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_3.png", _quantizer);

            var board = new Board(_initialBoard);
            var lines = _boardExtractor.MultiplayerRaisedLines(screenshot, board);
            var position = _boardExtractor.MultiplayerHolePosition(screenshot, board);

            var newBoard = _boardExtractor.MultiplayerAddLines(board, lines, position.Result);
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
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,1,1,1,0,1,1,1,1,1,
                1,1,1,1,0,1,1,1,1,1,
                1,1,1,1,1,1,1,1,0,1,
                1,1,1,1,1,1,1,1,0,1,
                1,1,1,1,1,1,1,1,0,1,
                1,1,1,1,1,1,1,1,0,1
            });

            Assert.AreEqual(4, lines);
            Assert.AreEqual(8, position.Result);
            Assert.AreEqual(expected, newBoard);
        }

        [Test]
        public void IsHorizonBroken()
        {
            var screenshot = TestHelper.GetScreenshot("Screenshots/tetris_multiplayer_3.png", _quantizer);
            var board = new Board(_initialBoard);
            var piece = new Piece(Tetrimino.Z).Rotate().Right().Right().Fall(4);

            var broken = _boardExtractor.IsHorizonBroken(screenshot, board);
            Assert.True(broken);

            var newBoard = _boardExtractor.Update(screenshot, board, piece);
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
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,1,1,1,0,1,1,1,1,1,
                1,1,1,1,0,1,1,1,1,1,
                1,1,1,1,1,1,1,1,0,1,
                1,1,1,1,1,1,1,1,0,1,
                1,1,1,1,1,1,1,1,0,1,
                1,1,1,1,1,1,1,1,0,1
            });

            Assert.AreEqual(expected, newBoard);
        }
    }
}
