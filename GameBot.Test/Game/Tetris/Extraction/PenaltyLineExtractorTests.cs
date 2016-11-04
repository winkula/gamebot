using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class PenaltyLineExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        [Test]
        public void Constructor()
        {
            var extractor = new PenaltyLineExtractor();

            var screenshot = new EmguScreenshot("Screenshots/abc.png", TimeSpan.Zero);
            var board = new Board();

            var newBoard = extractor.Update(screenshot, board);

            //Assert.True(newBoard.IsOccupied(3, 2));
        }
    }
}
