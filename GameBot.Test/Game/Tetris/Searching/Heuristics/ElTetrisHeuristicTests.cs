using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching.Heuristics;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Searching.Heuristics
{
    [TestFixture]
    public class ElTetrisHeuristicTests
    {
        private ElTetrisHeuristic _heuristic;

        [SetUp]
        public void Init()
        {
            _heuristic = new ElTetrisHeuristic();
        }

        [TestCase(50, new[] {
            0,0,0,0,0,1,0,0,0,0,
            0,0,1,0,0,1,1,0,0,0,
            0,1,1,1,0,1,1,0,0,1,
            1,1,1,1,0,1,1,0,0,1,
            1,1,1,1,1,1,1,0,0,1,
            1,1,1,0,1,1,1,1,1,1
        })]
        [TestCase(44, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,1,0,0,1,0,0,0,0,
            0,1,1,1,1,1,0,0,0,1,
            1,1,1,1,0,1,1,1,1,1,
            1,1,0,1,1,1,1,1,1,1
        })]
        public void RowTransitions(double expected, int[] squares)
        {
            var board = TestHelper.BuildBoard(10, 6, squares);

            var rowTransitions = _heuristic.RowTransitions(board);

            Assert.AreEqual(expected, rowTransitions);
        }

        [TestCase(12, new[] {
            0,0,0,0,0,1,0,0,0,0,
            0,0,1,0,0,1,1,0,0,0,
            0,1,1,1,0,1,1,0,0,1,
            1,1,1,1,0,1,1,0,0,1,
            1,1,1,1,1,1,1,0,0,1,
            1,1,1,0,1,1,1,1,1,1
        })]
        [TestCase(14, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,1,0,0,1,0,0,0,0,
            0,1,1,1,1,1,0,0,0,1,
            1,1,1,1,0,1,1,1,1,1,
            1,1,0,1,1,1,1,1,1,1
        })]
        public void ColumnTransitions(double expected, int[] squares)
        {
            var board = TestHelper.BuildBoard(10, 6, squares);

            var columnTransitions = _heuristic.ColumnTransitions(board);

            Assert.AreEqual(expected, columnTransitions);
        }

        [TestCase(1, new[] {
            0,0,0,0,0,1,0,0,0,0,
            0,0,1,0,0,1,1,0,0,0,
            0,1,1,1,0,1,1,0,0,1,
            1,1,1,1,0,1,1,0,0,1,
            1,1,1,1,1,1,1,0,0,1,
            1,1,1,0,1,1,1,1,1,1
        })]
        [TestCase(2, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,1,0,0,1,0,0,0,0,
            0,1,1,1,1,1,0,0,0,1,
            1,1,1,1,0,1,1,1,1,1,
            1,1,0,1,1,1,1,1,1,1
        })]
        public void Holes(double expected, int[] squares)
        {
            var board = TestHelper.BuildBoard(10, 6, squares);

            var holes = _heuristic.Holes(board);

            Assert.AreEqual(expected, holes);
        }

        [TestCase(5, new[] {
            0,0,0,0,0,1,0,0,0,0,
            0,0,1,0,0,1,1,0,0,0,
            0,1,1,1,0,1,1,0,0,1,
            1,1,1,1,0,1,1,0,0,1,
            1,1,1,1,1,1,1,0,0,1,
            1,1,1,0,1,1,1,1,1,1
        })]
        [TestCase(3, new[] {
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,1,0,0,1,0,0,0,0,
            0,1,1,1,1,1,0,0,0,1,
            1,1,1,1,0,1,1,1,1,1,
            1,1,0,1,1,1,1,1,1,1
        })]
        public void WellSums(double expected, int[] squares)
        {
            var board = TestHelper.BuildBoard(10, 6, squares);

            var wellSums = _heuristic.WellSums(board);

            Assert.AreEqual(expected, wellSums);
        }
    }
}
