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
        private PredictiveSearch _predictiveSearch;
        private RecursiveSearch _recursiveSearch;

        [TestFixtureSetUp]
        public void Init()
        {
            _heuristic = new YiyuanLeeHeuristic();

            _simpleSearch = new SimpleSearch(_heuristic);
            _predictiveSearch = new PredictiveSearch(_heuristic);
            _recursiveSearch = new RecursiveSearch(_heuristic);
            _recursiveSearch.Depth = 2;
        }

        [TestCase(Tetrimino.O, Tetrimino.S)]
        [TestCase(Tetrimino.I, Tetrimino.L)]
        [TestCase(Tetrimino.S, Tetrimino.Z)]
        [TestCase(Tetrimino.Z, Tetrimino.I)]
        [TestCase(Tetrimino.L, Tetrimino.O)]
        [TestCase(Tetrimino.J, Tetrimino.T)]
        [TestCase(Tetrimino.T, Tetrimino.J)]
        public void SimpleSearch(Tetrimino current, Tetrimino next)
        {
            var gameState = new GameState(current, next);
            
            var result = _simpleSearch.Search(gameState);
            _logger.Info(result.GoalGameState);
        }

        [TestCase(Tetrimino.T, Tetrimino.J)]
        public void SimpleSearchFull(Tetrimino current, Tetrimino next)
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

        [TestCase(Tetrimino.O, Tetrimino.S)]
        [TestCase(Tetrimino.I, Tetrimino.L)]
        [TestCase(Tetrimino.S, Tetrimino.Z)]
        [TestCase(Tetrimino.Z, Tetrimino.I)]
        [TestCase(Tetrimino.L, Tetrimino.O)]
        [TestCase(Tetrimino.J, Tetrimino.T)]
        [TestCase(Tetrimino.T, Tetrimino.J)]
        public void PredictiveSearch(Tetrimino current, Tetrimino next)
        {
            var gameState = new GameState(current, next);

            var result = _predictiveSearch.Search(gameState);
            _logger.Info(result.GoalGameState);
        }

        [TestCase(Tetrimino.O, Tetrimino.S)]
        [TestCase(Tetrimino.I, Tetrimino.L)]
        [TestCase(Tetrimino.S, Tetrimino.Z)]
        [TestCase(Tetrimino.Z, Tetrimino.I)]
        [TestCase(Tetrimino.L, Tetrimino.O)]
        [TestCase(Tetrimino.J, Tetrimino.T)]
        [TestCase(Tetrimino.T, Tetrimino.J)]
        public void RecursiveSearch(Tetrimino current, Tetrimino next)
        {
            var gameState = new GameState(current, next);

            var result = _recursiveSearch.Search(gameState);
            _logger.Info(result.GoalGameState);
        }
    }
}
