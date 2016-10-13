using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using NLog;
using NUnit.Framework;
using System.Linq;

namespace GameBot.Test.Tetris.Searching
{
    [TestFixture]
    public class TetrisSearchTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            logger.Info(result.GoalGameState);
        }

        [TestCase(Tetromino.T, Tetromino.J)]
        public void SimpleSearchFull(Tetromino current, Tetromino next)
        {
            var simpleSearchLocal = new SimpleSearch(new YiyuanLeeHeuristic());

            var gameState = new GameState(current, next);
            logger.Info(gameState);

            var result = simpleSearchLocal.Search(gameState);
            logger.Info(result.GoalGameState);

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
            logger.Info(result.GoalGameState);
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
            logger.Info(result.GoalGameState);
        }
    }
}
