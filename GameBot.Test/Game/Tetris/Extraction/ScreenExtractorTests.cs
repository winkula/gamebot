using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;
using GameBot.Core;
using GameBot.Core.Quantizers;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class ScreenExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private IScreenExtractor _screenExtractor;
        private IQuantizer _quantizer;

        [TestFixtureSetUp]
        public void Init()
        {
            _screenExtractor = new ScreenExtractor();
            _quantizer = new MorphologyQuantizer(TestHelper.GetFakeConfig().Object);
        }

        [TestCase("Screenshots/tetris_credits.png", false)]
        [TestCase("Screenshots/tetris_menu.png", false)]
        [TestCase("Screenshots/tetris_menu.png", false)]
        [TestCase("Screenshots/tetris_menu_atype.png", false)]
        [TestCase("Screenshots/tetris_multiplayer_0.png", false)]
        [TestCase("Screenshots/tetris_play_1.png", false)]
        [TestCase("Screenshots/tetris_play_2.png", false)]
        [TestCase("Screenshots/tetris_start.png", false)]
        [TestCase("Screenshots/white.png", false)]
        [TestCase("Screenshots/multiplayer_gameover.png", false)]

        [TestCase("Screenshots/gameover.png", true)]
        public void IsGameOverSingleplayer(string path, bool expected)
        {
            var screenshot = TestHelper.GetScreenshot(path, _quantizer);

            var isGameOver = _screenExtractor.IsGameOverSingleplayer(screenshot);

            Assert.AreEqual(expected, isGameOver);
        }

        [TestCase("Screenshots/tetris_credits.png", false)]
        [TestCase("Screenshots/tetris_menu.png", false)]
        [TestCase("Screenshots/tetris_menu.png", false)]
        [TestCase("Screenshots/tetris_menu_atype.png", false)]
        [TestCase("Screenshots/tetris_multiplayer_0.png", false)]
        [TestCase("Screenshots/tetris_play_1.png", false)]
        [TestCase("Screenshots/tetris_play_2.png", false)]
        [TestCase("Screenshots/tetris_start.png", false)]
        [TestCase("Screenshots/white.png", false)]
        [TestCase("Screenshots/gameover.png", false)]

        [TestCase("Screenshots/multiplayer_gameover.png", true)]
        public void IsGameOverMultiplayer(string path, bool expected)
        {
            var screenshot = TestHelper.GetScreenshot(path, _quantizer);

            var isGameOver = _screenExtractor.IsGameOverMultiplayer(screenshot);

            Assert.AreEqual(expected, isGameOver);
        }

        [TestCase("Screenshots/tetris_credits.png", false)]
        [TestCase("Screenshots/tetris_menu.png", false)]
        [TestCase("Screenshots/tetris_menu.png", false)]
        [TestCase("Screenshots/tetris_menu_atype.png", false)]
        [TestCase("Screenshots/tetris_start.png", false)]
        [TestCase("Screenshots/gameover.png", false)]
        [TestCase("Screenshots/multiplayer_gameover.png", false)]

        [TestCase("Screenshots/multiplayer_start.png", true)]
        [TestCase("Screenshots/tetris_play_1.png", true)]
        [TestCase("Screenshots/tetris_play_2.png", true)]
        public void IsStart(string path, bool expected)
        {
            var screenshot = TestHelper.GetScreenshot(path, _quantizer);

            var isStart = _screenExtractor.IsStart(screenshot);

            Assert.AreEqual(expected, isStart);
        }
    }
}
