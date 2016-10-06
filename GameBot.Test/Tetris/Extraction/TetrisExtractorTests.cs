using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Robot.Configuration;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Test.Tetris.Extraction
{
    [TestFixture]
    public class TetrisExtractorTests
    {
        [Test]
        public void Constructor()
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
        }

        [Test]
        public void Extract()
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
            var image = Image.FromFile("Screenshots/tetris_play_1.png");
            var screenshot = new EmguScreenshot(image, TimeSpan.Zero);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var gameState = extractor.Extract(screenshot, null);

            stopwatch.Stop();
            Debug.Write($"Extraction in {stopwatch.ElapsedMilliseconds} ms");

            Assert.NotNull(gameState);
            Assert.NotNull(gameState.Board);
            Assert.NotNull(gameState.Piece);
            Assert.NotNull(gameState.NextPiece);

            Debug.WriteLine(gameState.Piece);
            Debug.WriteLine(gameState.NextPiece);

            Debug.WriteLine(gameState);
            Debug.WriteLine(gameState.Board);
        }

        [Test]
        public void ExtractSpawnedPieceOrigin()
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
            var image = Image.FromFile("Screenshots/tetris_play_1.png");
            var screenshot = new EmguScreenshot(image, TimeSpan.Zero);

            var piece = extractor.ExtractSpawnedPieceOrigin(screenshot);

            Assert.NotNull(piece);
            Assert.AreEqual(Tetromino.S, piece.Tetromino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(0, piece.Y);
        }

        [Test]
        public void ExtractPieceSpawnedFalled()
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
            var image = Image.FromFile("Screenshots/tetris_play_2.png");
            var screenshot = new EmguScreenshot(image, TimeSpan.Zero);

            var piece = extractor.ExtractSpawnedPiece(screenshot, 5);

            Assert.Null(piece);

            piece = extractor.ExtractSpawnedPiece(screenshot, 6);

            Assert.NotNull(piece);
            Assert.AreEqual(Tetromino.Z, piece.Tetromino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(-6, piece.Y);
        }

        // TODO: fix this
        [TestCase(0, 0, 0x0000)]
        [TestCase(0, -5, 0x0003)]
        [TestCase(0, -6, 0x0036)]
        public void GetPieceMask(int x, int y, int expected)
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
            var image = Image.FromFile("Screenshots/tetris_play_2.png");
            var screenshot = new EmguScreenshot(image, TimeSpan.Zero);

            var mask = extractor.GetPieceMask(screenshot, x, y);
            Assert.AreEqual(expected, (ushort)mask);
        }
    }
}
