﻿using GameBot.Game.Tetris.Data;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris
{
    [TestFixture]
    public class GameStateTests
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        [TestCase(Tetromino.L, Tetromino.O, 0, 10, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,2,2,2,0,0,0,0,
            0,0,0,2,0,0,0,0,0,0,
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
            0,0,1,2,2,2,0,0,0,0,
            0,0,1,2,0,1,0,0,0,0,
            0,1,1,0,0,1,1,0,0,0,
            0,1,1,1,0,1,1,0,1,0,
            1,1,1,1,0,1,1,0,1,0,
            0,1,1,1,0,0,1,0,1,0,
            1,1,0,1,0,0,1,0,1,0
        })]
        [TestCase(Tetromino.S, Tetromino.I, -3, 13, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,2,2,0,0,0,0,0,0,0,
            2,2,0,0,0,0,0,0,0,0,
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
            1,1,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0
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
            0,0,0,0,0,0,0,0,0,0,
            0,2,2,0,0,0,0,0,0,0,
            2,2,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0
        })]
        public void Drop(Tetromino piece, Tetromino next, int translation, int expectedFall, int[] before, int[] after)
        {
            var gameState = new GameState(new Piece(piece, 0, translation), next);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    if (before[10 * (18 - 1 - y) + x] == 1) { gameState.Board.Occupy(x, y); }
                }
            }

            Assert.AreEqual(piece, gameState.Piece.Tetromino);
            Assert.AreEqual(0, gameState.Board.Pieces);

            int fall = gameState.Drop();

            Assert.AreEqual(expectedFall, fall);

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    if (after[10 * (18 - 1 - y) + x] > 0)
                    {
                        Assert.True(gameState.Board.IsOccupied(x, y));
                        Assert.False(gameState.Board.IsFree(x, y));
                    }
                    else
                    {
                        Assert.False(gameState.Board.IsOccupied(x, y));
                        Assert.True(gameState.Board.IsFree(x, y));
                    }
                }
            }

            Assert.AreEqual(next, gameState.Piece.Tetromino);
            Assert.AreEqual(1, gameState.Board.Pieces);
        }
        
        [Test]
        public void StringRepresentation()
        {
            var gameState = new GameState();

            var str = gameState.ToString();
            logger.Info(str);

            Assert.NotNull(str);
        }
    }
}
