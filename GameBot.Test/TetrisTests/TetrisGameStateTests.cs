using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisGameStateTests
    {
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
            var gameState = new TetrisGameState(new Piece(piece, 0, translation), new Piece(next));
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
    }
}
