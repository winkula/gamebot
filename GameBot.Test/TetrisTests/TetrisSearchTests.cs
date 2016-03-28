using GameBot.Core.Searching;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test
{
    [TestFixture]
    public class TetrisSearchTests
    {
        private IHeuristic<TetrisGameState> heuristic;
        private TetrisSearch search;

        [SetUp]
        public void Setup()
        {
            heuristic = new TetrisHeuristic();
            search = new TetrisSearch(heuristic);
        }

        [TestCase(Tetromino.O, Tetromino.S)]
        [TestCase(Tetromino.I, Tetromino.L)]
        [TestCase(Tetromino.S, Tetromino.Z)]
        [TestCase(Tetromino.Z, Tetromino.I)]
        [TestCase(Tetromino.L, Tetromino.O)]
        [TestCase(Tetromino.J, Tetromino.T)]
        [TestCase(Tetromino.T, Tetromino.J)]
        public void Search(Tetromino current, Tetromino next)
        {
            var gameState = new TetrisGameState(current, next);
            var node = new TetrisNode(gameState);
            
            var result = search.Search(node);
            Debug.WriteLine(result.GameState);
        }
    }
}
