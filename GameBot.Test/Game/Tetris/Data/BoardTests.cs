﻿using GameBot.Game.Tetris.Data;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Data
{
    [TestFixture]
    public class BoardTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void Constructor()
        {
            int width = 3;
            int height = 7;

            var board = new Board(width, height);

            Assert.AreEqual(width, board.Width);
            Assert.AreEqual(height, board.Height);
            Assert.AreEqual(0, board.Pieces);

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    Assert.True(board.IsFree(x, y));
                    Assert.False(board.IsOccupied(x, y));
                }
            }
        }

        [Test]
        public void ConstructorDefault()
        {
            var board = new Board();

            Assert.AreEqual(10, board.Width);
            Assert.AreEqual(19, board.Height);
            Assert.AreEqual(0, board.Pieces);

            _logger.Info(board.ToString());

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    Assert.True(board.IsFree(x, y));
                    Assert.False(board.IsOccupied(x, y));
                }
            }
        }

        [Test]
        public void SquareExists()
        {
            var board = new Board();

            Assert.True(board.SquareExists(0, 0));
            Assert.True(board.SquareExists(board.Width - 1, board.Height - 1));
            Assert.True(board.SquareExists(board.Width / 2, board.Height / 2));
            Assert.False(board.SquareExists(board.Width, board.Height));
            Assert.False(board.SquareExists(0, board.Height));
            Assert.False(board.SquareExists(board.Width, 0));
            Assert.False(board.SquareExists(-1, -1));
            Assert.False(board.SquareExists(-1, 0));
            Assert.False(board.SquareExists(0, -1));
        }

        [TestCase(1, 5)]
        [TestCase(3, 12)]
        [TestCase(9, 2)]
        [TestCase(5, 17)]
        [TestCase(0, 0)]
        [TestCase(9, 18)]
        public void Occupy(int posX, int posY)
        {
            var board = new Board();

            Assert.True(board.IsFree(posX, posY));

            board.Occupy(posX, posY);

            Assert.True(board.IsOccupied(posX, posY));

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    if (x != posX || y != posY)
                    {
                        Assert.True(board.IsFree(x, y));
                    }
                }
            }
        }

        [TestCase(1, 5)]
        [TestCase(3, 12)]
        [TestCase(9, 2)]
        [TestCase(5, 17)]
        [TestCase(0, 0)]
        [TestCase(9, 18)]
        public void Free(int posX, int posY)
        {
            var board = TestHelper.BuildBoard(new[] {
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1
            });

            Assert.True(board.IsOccupied(posX, posY));

            board.Free(posX, posY);

            Assert.True(board.IsFree(posX, posY));

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    _logger.Info(x + "," + y);
                    if (x != posX || y != posY)
                    {
                        Assert.True(board.IsOccupied(x, y));
                    }
                }
            }
        }

        [Test]
        public void Place()
        {
            var expected = TestHelper.BuildBoard(new[] {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
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
                0,0,0,0,0,0,0,0,0,0
            });
            var board = new Board();
            var piece = new Piece(Tetrimino.I).Rotate().Fall();

            board.Place(piece);

            Assert.AreEqual(10, board.Width);
            Assert.AreEqual(19, board.Height);
            Assert.AreEqual(1, board.Pieces);
            Assert.True(SquaresEqual(expected, board));
        }

        [Test]
        public void CompletedLines()
        {
            var board = TestHelper.BuildBoard(new[] {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                1,1,1,1,1,1,1,1,1,1,
                0,0,0,0,0,1,0,0,0,0,
                0,1,1,1,1,1,1,1,1,1,
                0,0,1,0,1,1,0,0,1,0,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1
            });

            Assert.AreEqual(4, board.CompletedLines);
        }

        [Test]
        public void ColumnHeight()
        {
            var board = TestHelper.BuildBoard(new[] {
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
                0,0,1,0,0,0,0,0,0,0,
                0,0,1,0,0,1,0,0,0,0,
                0,1,1,0,0,1,1,0,0,0,
                0,1,1,1,0,1,1,0,1,0,
                1,1,1,1,0,1,1,0,1,0,
                0,1,1,1,0,0,1,0,1,0,
                1,1,0,1,0,0,1,0,1,0
            });

            Assert.AreEqual(3, board.ColumnHeight(0));
            Assert.AreEqual(5, board.ColumnHeight(1));
            Assert.AreEqual(7, board.ColumnHeight(2));
            Assert.AreEqual(4, board.ColumnHeight(3));
            Assert.AreEqual(0, board.ColumnHeight(4));
            Assert.AreEqual(6, board.ColumnHeight(5));
            Assert.AreEqual(5, board.ColumnHeight(6));
            Assert.AreEqual(0, board.ColumnHeight(7));
            Assert.AreEqual(4, board.ColumnHeight(8));
            Assert.AreEqual(0, board.ColumnHeight(9));

            Assert.AreEqual(7, board.MaximumHeight);
        }

        [Test]
        public void ColumnHoles()
        {
            var board = TestHelper.BuildBoard(new[] {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,1,0,0,0,0,0,0,1,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,1,0,0,0,0,0,0,0,
                0,0,1,0,0,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,1,1,0,0,0,0,0,0,
                0,0,1,1,1,1,1,1,0,0,
                0,1,1,1,0,1,1,0,1,0,
                0,1,1,1,0,1,1,0,0,0,
                1,1,1,1,0,1,1,1,1,0,
                0,1,1,1,0,0,1,0,1,0,
                1,1,0,1,0,0,1,0,1,0
            });

            Assert.AreEqual(1, board.ColumnHoles(0));
            Assert.AreEqual(0, board.ColumnHoles(1));
            Assert.AreEqual(5, board.ColumnHoles(2));
            Assert.AreEqual(0, board.ColumnHoles(3));
            Assert.AreEqual(7, board.ColumnHoles(4));
            Assert.AreEqual(2, board.ColumnHoles(5));
            Assert.AreEqual(0, board.ColumnHoles(6));
            Assert.AreEqual(4, board.ColumnHoles(7));
            Assert.AreEqual(1, board.ColumnHoles(8));
            Assert.AreEqual(13, board.ColumnHoles(9));
        }

        [Test]
        public void RemoveLines1()
        {
            var board = TestHelper.BuildBoard(new[] {
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
                0,0,1,0,0,1,0,0,0,0,
                1,1,1,1,0,1,1,1,1,1,
                0,0,1,0,0,1,0,0,1,0,
                1,1,1,1,0,1,1,1,1,1,
                1,0,0,1,0,1,0,1,0,0,
                0,1,0,1,1,1,1,0,0,0,
                1,0,1,1,0,1,0,0,1,0,
                0,0,1,1,0,1,1,1,0,1
            });
            var expected = TestHelper.BuildBoard(new[] {
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
                0,0,1,0,0,1,0,0,0,0,
                0,0,1,0,1,1,0,0,1,0,
                1,0,0,1,1,1,0,1,0,0,
                0,1,0,1,1,1,1,0,0,0,
                1,0,1,1,0,1,0,0,1,0,
                0,0,1,1,0,1,1,1,0,1
            });
            var piece = new Piece(Tetrimino.I).Rotate().Fall(12);

            board.Place(piece);
            board.RemoveLines();

            Assert.True(SquaresEqual(expected, board));
        }

        [Test]
        public void RemoveLines2()
        {
            var board = TestHelper.BuildBoard(new[] {
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
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                1,1,1,1,0,1,1,1,1,1
            });
            var expected = TestHelper.BuildBoard(new[] {
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
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0,
                0,0,0,0,1,0,0,0,0,0
            });
            var piece = new Piece(Tetrimino.I).Rotate().Fall(15);

            board.Place(piece);
            board.RemoveLines();

            _logger.Info(board.ToString());
            _logger.Info(expected.ToString());

            Assert.True(SquaresEqual(expected, board));
        }

        [Test]
        public void Intersects()
        {
            var board = TestHelper.BuildBoard(new[] {
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,0,0,0,1,0,0,0,0,
                0,0,1,0,0,1,0,0,0,0,
                0,0,1,0,1,1,0,0,1,0,
                1,0,0,1,1,1,0,1,0,0,
                0,1,0,1,1,1,1,0,0,0,
                1,0,1,1,0,1,0,0,1,0,
                0,0,1,1,0,1,1,1,0,1
            });
            var piece = new Piece(Tetrimino.O).Fall(6);

            Assert.True(board.Intersects(piece));
        }

        [Test]
        public void NotIntersects()
        {
            var board = new Board();
            var piece = new Piece(Tetrimino.Z).Rotate();

            Assert.False(board.Intersects(piece));
        }

        [Test]
        public void Equal()
        {
            var board1 = new Board();
            var board2 = new Board();

            board1.Occupy(1, 4);
            board2.Occupy(1, 4);

            board1.Occupy(5, 11);
            board2.Occupy(5, 11);

            Assert.AreEqual(board1, board2);
        }

        [Test]
        public void NotEqual()
        {
            var board1 = new Board();
            var board2 = new Board();

            board1.Occupy(1, 4);
            board2.Occupy(1, 4);

            board1.Occupy(5, 9);
            board2.Occupy(6, 10);

            Assert.AreNotEqual(board1, board2);
        }

        [Test]
        public void StringRepresentation()
        {
            var board = new Board();

            var str = board.ToString();
            _logger.Info(str);

            Assert.NotNull(str);
        }

        [TestCase(1, 0, new[] {
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
            0,0,1,0,0,1,0,0,0,0,
            0,0,1,0,1,1,0,0,1,0,
            1,0,0,1,1,1,0,1,0,0,
            0,1,0,1,1,1,1,0,0,0,
            1,0,1,1,0,1,0,0,1,0,
            0,0,1,1,0,1,1,1,0,1
        }, new[] {
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
            0,0,1,0,0,1,0,0,0,0,
            0,0,1,0,1,1,0,0,1,0,
            1,0,0,1,1,1,0,1,0,0,
            0,1,0,1,1,1,1,0,0,0,
            1,0,1,1,0,1,0,0,1,0,
            0,0,1,1,0,1,1,1,0,1,
            0,1,1,1,1,1,1,1,1,1
        })]
        [TestCase(0, 0, new[] {
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
            0,0,1,0,0,1,0,0,0,0,
            0,0,1,0,1,1,0,0,1,0,
            1,0,0,1,1,1,0,1,0,0,
            0,1,0,1,1,1,1,0,0,0,
            1,0,1,1,0,1,0,0,1,0,
            0,0,1,1,0,1,1,1,0,1
        }, new[] {
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
            0,0,1,0,0,1,0,0,0,0,
            0,0,1,0,1,1,0,0,1,0,
            1,0,0,1,1,1,0,1,0,0,
            0,1,0,1,1,1,1,0,0,0,
            1,0,1,1,0,1,0,0,1,0,
            0,0,1,1,0,1,1,1,0,1
        })]

        [TestCase(4, 5, new[] {
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
            0,0,1,0,0,1,0,0,0,0,
            0,0,1,0,1,1,0,0,1,0,
            1,0,0,1,1,1,0,1,0,0,
            0,1,0,1,1,1,1,0,0,0,
            1,0,1,1,0,1,0,0,1,0,
            0,0,1,1,0,1,1,1,0,1
        }, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,1,0,0,1,0,0,0,0,
            0,0,1,0,1,1,0,0,1,0,
            1,0,0,1,1,1,0,1,0,0,
            0,1,0,1,1,1,1,0,0,0,
            1,0,1,1,0,1,0,0,1,0,
            0,0,1,1,0,1,1,1,0,1,
            1,1,1,1,1,0,1,1,1,1,
            1,1,1,1,1,0,1,1,1,1,
            1,1,1,1,1,0,1,1,1,1,
            1,1,1,1,1,0,1,1,1,1
        })]
        public void InsertLinesBottom(int numLines, int holePosition, int[] initialBoard, int[] expectedBoard)
        {
            var board = TestHelper.BuildBoard(initialBoard);
            var expected = TestHelper.BuildBoard(expectedBoard);
            var gameState = new GameState(board);

            gameState.SpawnLines(numLines, holePosition);

            Assert.AreEqual(expected, board);
        }

        private bool SquaresEqual(Board expected, Board board)
        {
            for (int x = 0; x < expected.Width; x++)
            {
                for (int y = 0; y < expected.Height; y++)
                {
                    if (expected.IsOccupied(x, y) && !board.IsOccupied(x, y))
                    {
                        return false;
                    }
                    if (expected.IsFree(x, y) && !board.IsFree(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
