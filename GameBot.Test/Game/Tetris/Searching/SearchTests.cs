using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using NLog;
using NUnit.Framework;
using System.Linq;

namespace GameBot.Test.Game.Tetris.Searching
{
    [TestFixture]
    public class TetrisSearchTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IHeuristic _heuristic;

        private SimpleSearch _simpleSearch;
        private ProbabilisticSearch _probabilisticSearch;
        private RecursiveSearch _recursiveSearch;

        [TestFixtureSetUp]
        public void Init()
        {
            _heuristic = new YiyuanLeeHeuristic();

            _simpleSearch = new SimpleSearch(_heuristic);
            _probabilisticSearch = new ProbabilisticSearch(_heuristic);
            _recursiveSearch = new RecursiveSearch(_heuristic);
            _recursiveSearch.Depth = 2;
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
            
            var result = _simpleSearch.Search(gameState);
            _logger.Info(result.GoalGameState);
        }

        [TestCase(Tetromino.T, Tetromino.J)]
        public void SimpleSearchFull(Tetromino current, Tetromino next)
        {
            var simpleSearchLocal = new SimpleSearch(new YiyuanLeeHeuristic());

            var gameState = new GameState(current, next);
            _logger.Info(gameState);

            var result = simpleSearchLocal.Search(gameState);
            _logger.Info(result.GoalGameState);

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

            var result = _probabilisticSearch.Search(gameState);
            _logger.Info(result.GoalGameState);
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

            var result = _recursiveSearch.Search(gameState);
            _logger.Info(result.GoalGameState);
        }
    }
}
