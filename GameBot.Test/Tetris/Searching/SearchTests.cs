using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using NUnit.Framework;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Test.Tetris.Searching
{
    [TestFixture]
    public class TetrisSearchTests
    {
        private IHeuristic heuristic;

        private SimpleSearch simpleSearch;
        private ProbabilisticSearch probabilisticSearch;
        private RecursiveSearch recursiveSearch;

        [TestFixtureSetUp]
        public void Init()
        {
            heuristic = new YiyuanLeeHeuristic();

            simpleSearch = new SimpleSearch(heuristic);
            probabilisticSearch = new ProbabilisticSearch(heuristic);
            recursiveSearch = new RecursiveSearch(heuristic, 2);
        }

        [TestCase(Tetromino.O, Tetromino.S)]
        [TestCase(Tetromino.I, Tetromino.L)]
        [TestCase(Tetromino.S, Tetromino.Z)]
        [TestCase(Tetromino.Z, Tetromino.I)]
        [TestCase(Tetromino.L, Tetromino.O)]
        [TestCase(Tetromino.J, Tetromino.T)]
        [TestCase(Tetromino.T, Tetromino.J)]
        public void SimpleSearch(Tetromino current, Tetromino next)
        {
            var gameState = new GameState(current, next);
            
            var result = simpleSearch.Search(gameState);
            Debug.WriteLine(result.GoalGameState);
        }

        [TestCase(Tetromino.T, Tetromino.J)]
        public void SimpleSearchFull(Tetromino current, Tetromino next)
        {
            var simpleSearchLocal = new SimpleSearch(new YiyuanLeeHeuristic());

            var gameState = new GameState(current, next);
            Debug.WriteLine(gameState);

            var result = simpleSearchLocal.Search(gameState);
            Debug.WriteLine(result.GoalGameState);

            Assert.AreEqual(3, result.Moves.Count());
            Assert.AreEqual(Move.Rotate, result.Moves.ToList()[0]);
            Assert.AreEqual(Move.Rotate, result.Moves.ToList()[1]);
            Assert.AreEqual(Move.Drop, result.Moves.ToList()[2]);
        }

        [TestCase(Tetromino.O, Tetromino.S)]
        [TestCase(Tetromino.I, Tetromino.L)]
        [TestCase(Tetromino.S, Tetromino.Z)]
        [TestCase(Tetromino.Z, Tetromino.I)]
        [TestCase(Tetromino.L, Tetromino.O)]
        [TestCase(Tetromino.J, Tetromino.T)]
        [TestCase(Tetromino.T, Tetromino.J)]
        public void ProbabilisticSearch(Tetromino current, Tetromino next)
        {
            var gameState = new GameState(current, next);

            var result = probabilisticSearch.Search(gameState);
            Debug.WriteLine(result.GoalGameState);
        }

        [TestCase(Tetromino.O, Tetromino.S)]
        [TestCase(Tetromino.I, Tetromino.L)]
        [TestCase(Tetromino.S, Tetromino.Z)]
        [TestCase(Tetromino.Z, Tetromino.I)]
        [TestCase(Tetromino.L, Tetromino.O)]
        [TestCase(Tetromino.J, Tetromino.T)]
        [TestCase(Tetromino.T, Tetromino.J)]
        public void RecursiveSearch(Tetromino current, Tetromino next)
        {
            var gameState = new GameState(current, next);

            var result = recursiveSearch.Search(gameState);
            Debug.WriteLine(result.GoalGameState);
        }
    }
}
