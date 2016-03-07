using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Test
{
    [TestFixture]
    public class PieceTests
    {
        [Test]
        public void Constructor()
        {
            var tetromino = Tetromino.S;
            var orientation = 1;
            var x = 1;
            var y = -4;

            var piece = new Piece(tetromino, orientation, x, y);

            Assert.AreEqual(tetromino, piece.Tetromino);
            Assert.AreEqual(orientation, piece.Orientation);
            Assert.AreEqual(x, piece.X);
            Assert.AreEqual(y, piece.Y);
        }

        [Test]
        public void ConstructorDefault()
        {
            var tetromino = Tetromino.S;

            var piece = new Piece(tetromino);

            Assert.AreEqual(tetromino, piece.Tetromino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(0, piece.Y);
        }

        [TestCase(Tetromino.O, new[] { 0, 1, 2, 3 }, new[] {
            0,0,0,0,
            0,0,0,0,
            0,1,1,0,
            0,1,1,0
        })]
        [TestCase(Tetromino.I, new[] { 0, 2 }, new[] {
            0,0,0,0,
            0,0,0,0,
            1,1,1,1,
            0,0,0,0
        })]
        [TestCase(Tetromino.I, new[] { 1, 3 }, new[] {
            0,1,0,0,
            0,1,0,0,
            0,1,0,0,
            0,1,0,0
        })]
        [TestCase(Tetromino.S, new[] { 0, 2 }, new[] {
            0,0,0,0,
            0,0,0,0,
            0,1,1,0,
            1,1,0,0
        })]
        [TestCase(Tetromino.S, new[] { 1, 3 }, new[] {
            0,0,0,0,
            1,0,0,0,
            1,1,0,0,
            0,1,0,0
        })]
        [TestCase(Tetromino.Z, new[] { 0, 2 }, new[] {
            0,0,0,0,
            0,0,0,0,
            1,1,0,0,
            0,1,1,0
        })]
        [TestCase(Tetromino.Z, new[] { 1, 3 }, new[] {
            0,0,0,0,
            0,1,0,0,
            1,1,0,0,
            1,0,0,0
        })]
        [TestCase(Tetromino.L, new[] { 0 }, new[] {
            0,0,0,0,
            1,1,0,0,
            0,1,0,0,
            0,1,0,0
        })]
        [TestCase(Tetromino.L, new[] { 1 }, new[] {
            0,0,0,0,
            0,0,1,0,
            1,1,1,0,
            0,0,0,0
        })]
        [TestCase(Tetromino.L, new[] { 2 }, new[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,0,
            0,1,1,0
        })]
        [TestCase(Tetromino.L, new[] { 3 }, new[] {
            0,0,0,0,
            0,0,0,0,
            1,1,1,0,
            1,0,0,0
        })]
        [TestCase(Tetromino.J, new[] { 0 }, new[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,0,
            1,1,0,0
        })]
        [TestCase(Tetromino.J, new[] { 1 }, new[] {
            0,0,0,0,
            1,0,0,0,
            1,1,1,0,
            0,0,0,0
        })]
        [TestCase(Tetromino.J, new[] { 2 }, new[] {
            0,0,0,0,
            0,1,1,0,
            0,1,0,0,
            0,1,0,0
        })]
        [TestCase(Tetromino.J, new[] { 3 }, new[] {
            0,0,0,0,
            0,0,0,0,
            1,1,1,0,
            0,0,1,0
        })]
        [TestCase(Tetromino.T, new[] { 0 }, new[] {
            0,0,0,0,
            0,0,0,0,
            1,1,1,0,
            0,1,0,0
        })]
        [TestCase(Tetromino.T, new[] { 1 }, new[] {
            0,0,0,0,
            0,1,0,0,
            1,1,0,0,
            0,1,0,0
        })]
        [TestCase(Tetromino.T, new[] { 2 }, new[] {
            0,0,0,0,
            0,1,0,0,
            1,1,1,0,
            0,0,0,0
        })]
        [TestCase(Tetromino.T, new[] { 3 }, new[] {
            0,0,0,0,
            0,1,0,0,
            0,1,1,0,
            0,1,0,0
        })]
        public void IsSquareOccupied(Tetromino tetromino, int[] orientations, int[] fields)
        {
            foreach (int orientation in orientations)
            {
                Piece piece = new Piece(tetromino, orientation);

                // out of bounds
                Assert.AreEqual(false, piece.IsSquareOccupied(Piece.CoordinateMin - 1, Piece.CoordinateMin - 1));
                
                for (int x = Piece.CoordinateMin; x <= Piece.CoordinateMax; x++)
                {
                    for (int y = Piece.CoordinateMin; y <= Piece.CoordinateMax; y++)
                    {
                        bool expected = fields[Piece.CoordinateSize * (Piece.CoordinateSize - 1 - (y - Piece.CoordinateMin)) + (x - Piece.CoordinateMin)] == 1;
                        bool occupied = piece.IsSquareOccupied(x, y);
                        Assert.AreEqual(expected, occupied);                        
                    }
                }
                
                // out of bounds
                Assert.AreEqual(false, piece.IsSquareOccupied(Piece.CoordinateMax + 1, Piece.CoordinateMax + 1));
            }
        }
    }
}
