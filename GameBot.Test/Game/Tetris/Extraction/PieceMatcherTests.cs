using System;
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
        
        //[TestCase("Screenshots/tetris_play_1.png", Tetromino.O, 0, 1, -2, 0.31)]
        //[TestCase("Screenshots/tetris_play_1.png", Tetromino.S, 0, 0, 0, 0.99)]
        //[TestCase("Screenshots/tetris_play_2.png", Tetromino.Z, 0, 0, -6, 0.99)]
        //[TestCase("Screenshots/tetris_play_2.png", Tetromino.Z, 0, 0, -5, 0.49)]
        //public void GetProbability(string file, Tetromino tetromino, int orientation, int x, int y, double expectedProbability)
        //{
        //    var screenshot = new EmguScreenshot(file, TimeSpan.Zero);
        //    var piece = new Piece(tetromino, orientation, x, y);
            
        //    var probability = _pieceMatcher.GetProbability(screenshot, piece);

        //    Assert.GreaterOrEqual(probability, expectedProbability);
        //}

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
                        var piece = new Piece(Tetromino.T, orientation, x, y);

                        var probability = _pieceMatcher.GetProbability(screenshot, piece);

                        Assert.GreaterOrEqual(probability, 0.0);
                        Assert.LessOrEqual(probability, 1.0);
                    }
                }
            }
        }
    }
}
