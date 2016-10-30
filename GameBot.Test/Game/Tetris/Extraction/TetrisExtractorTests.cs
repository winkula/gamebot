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
    public class TetrisExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        [Test]
        public void Constructor()
        {
            var config = new AppSettingsConfig();

            var extractor = new TetrisExtractor(config);
        }

        [Test]
        public void Extract()
        {
            var config = new AppSettingsConfig();

            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", DateTime.Now.Subtract(DateTime.MinValue));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var gameState = extractor.Extract(screenshot, null);

            stopwatch.Stop();
            Debug.Write($"Extraction in {stopwatch.ElapsedMilliseconds} ms");

            Assert.NotNull(gameState);
            Assert.NotNull(gameState.Board);
            Assert.NotNull(gameState.Piece);
            Assert.NotNull(gameState.NextPiece);

            _logger.Info(gameState.Piece);
            _logger.Info(gameState.NextPiece);

            _logger.Info(gameState);
            _logger.Info(gameState.Board);
        }

        [Test]
        public void ExtractSpawnedPieceOrigin()
        {
            var config = new AppSettingsConfig();
            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", DateTime.Now.Subtract(DateTime.MinValue));

            var piece = extractor.ExtractSpawnedPieceOrigin(screenshot);

            Assert.NotNull(piece);
            Assert.AreEqual(Tetrimino.S, piece.Tetrimino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(0, piece.Y);
        }
        
        [Test]
        public void ExtractPieceSpawnedFalled()
        {
            var config = new AppSettingsConfig();
            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", DateTime.Now.Subtract(DateTime.MinValue));

            var piece = extractor.ExtractSpawnedPiece(screenshot, 5);

            Assert.Null(piece);

            piece = extractor.ExtractSpawnedPiece(screenshot, 6);

            Assert.NotNull(piece);
            Assert.AreEqual(Tetrimino.Z, piece.Tetrimino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(-6, piece.Y);
        }

        [Test]
        public void ExtractPieceSpawnedFalledPieceIsInOrigin()
        {
            var config = new AppSettingsConfig();
            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", DateTime.Now.Subtract(DateTime.MinValue));

            var piece = extractor.ExtractSpawnedPiece(screenshot, 10);

            Assert.NotNull(piece);
            Assert.AreEqual(Tetrimino.S, piece.Tetrimino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(0, piece.Y);
        }

        [TestCase(0, 0, 0x0000)]
        [TestCase(0, -5, 0x0003)]
        [TestCase(0, -6, 0x0036)]
        [TestCase(-2, -8, 0xC800)]
        public void GetPieceMask(int x, int y, int expected)
        {
            var config = new AppSettingsConfig();

            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", DateTime.Now.Subtract(DateTime.MinValue));

            var mask = extractor.GetPieceMask(screenshot, x, y);

            Assert.AreEqual(expected, mask);
        }

        [Test]
        public void ConfirmPieceMovement()
        {
            var config = new AppSettingsConfig();

            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", DateTime.Now.Subtract(DateTime.MinValue));

            var lastPosition = new Piece(Tetrimino.Z, 0, 1, -6);
            
            var newPosition = extractor.ExtractMovedPieceWithoutErrorTolerance(screenshot, lastPosition, Move.Left, 0);

            Assert.NotNull(newPosition);

            Assert.AreEqual(Tetrimino.Z, newPosition.Tetrimino);
            Assert.AreEqual(0, newPosition.Orientation);
            Assert.AreEqual(0, newPosition.X);
            Assert.AreEqual(-6, newPosition.Y);
        }

        [Test]
        public void ConfirmPieceMovementWithFallDistance()
        {
            var config = new AppSettingsConfig();

            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", DateTime.Now.Subtract(DateTime.MinValue));

            var lastPosition = new Piece(Tetrimino.Z, 0, 1, 0);

            var newPosition = extractor.ExtractMovedPieceWithoutErrorTolerance(screenshot, lastPosition, Move.Left, 5);
            Assert.Null(newPosition);

            newPosition = extractor.ExtractMovedPieceWithoutErrorTolerance(screenshot, lastPosition, Move.Left, 6);
            Assert.NotNull(newPosition);

            Assert.AreEqual(Tetrimino.Z, newPosition.Tetrimino);
            Assert.AreEqual(0, newPosition.Orientation);
            Assert.AreEqual(0, newPosition.X);
            Assert.AreEqual(-6, newPosition.Y);
        }

        [TestCase(Tetrimino.Z, 0, -6, 1.0)]
        [TestCase(Tetrimino.Z, 0, -5, 1.0 - (4 / 16.0))]
        [TestCase(Tetrimino.Z, 0, -4, 1.0 - (4 / 16.0))]
        [TestCase(Tetrimino.Z, -1, -6, 1.0 - (4 / 16.0))]
        public void GetProbability(Tetrimino tetrimino, int x, int y, double expectedProbability)
        {
            var config = new AppSettingsConfig();

            var extractor = new TetrisExtractor(config);
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", DateTime.Now.Subtract(DateTime.MinValue));

            var piece = new Piece(tetrimino, 0, x, y);

            var probability = extractor.GetProbability(screenshot, piece);

            Assert.LessOrEqual(probability, 1.0);
            Assert.GreaterOrEqual(probability, 0.0);
            Assert.AreEqual(expectedProbability, probability);
        }
    }
}
