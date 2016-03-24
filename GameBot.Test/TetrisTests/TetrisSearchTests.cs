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

        [TestCase(Tetromino.O, Tetromino.Z)]
        [TestCase(Tetromino.I, Tetromino.I)]
        [TestCase(Tetromino.T, Tetromino.S)]
        public void Search(Tetromino current, Tetromino next)
        {
            var gameState = new TetrisGameState(current, next);
            var node = new TetrisNode(gameState);
            
            var result = search.Search(node);
            Debug.WriteLine(result.GameState);
        }
    }
}
