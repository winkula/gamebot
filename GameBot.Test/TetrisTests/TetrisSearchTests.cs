using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Heuristics;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisSearchTests
    {
        private TetrisSearch search;

        [SetUp]
        public void Setup()
        {
            //search = new TetrisSearch(new TetrisSurviveHeuristic());
            search = new TetrisSearch(new TetrisStackingHeuristic());
            //search = new TetrisSearch(new TetrisHolesHeuristic());
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
            var gameState = new TetrisGameState(new Piece(current), new Piece(next));
            var node = new TetrisNode(gameState);
            
            var result = search.Search(node);
            Debug.WriteLine(result.GameState);
        }
    }
}
