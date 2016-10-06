using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.Tetris.Searching
{
    [TestFixture]
    public class TetrisSearchTests
    {
        private IHeuristic heuristic;

        private SimpleSearch simpleSearch;
        private ProbabilisticSearch probabilisticSearch;
        private RecursiveSearch recursiveSearch;

        [SetUp]
        public void Setup()
        {
            heuristic = new YiyuanLeeHeuristic();

            simpleSearch = new SimpleSearch(heuristic);
            probabilisticSearch = new ProbabilisticSearch(heuristic);
            recursiveSearch = new RecursiveSearch(heuristic, 3);
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
            var gameState = new TetrisGameState(current, next);
            var node = new Node(gameState);
            
            var result = simpleSearch.Search(node);
            Debug.WriteLine(result.GoalGameState);
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
            var gameState = new TetrisGameState(current, next);
            var node = new Node(gameState);

            var result = probabilisticSearch.Search(node);
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
            var gameState = new TetrisGameState(current, next);
            var node = new Node(gameState);

            var result = recursiveSearch.Search(node);
            Debug.WriteLine(result.GoalGameState);
        }
    }
}
