using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Heuristics;
using NUnit.Framework;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisSurviveHeuristicTests
    {
        [Test]
        public void Constructor()
        {
            var heuristic = new YiyuanLeeHeuristic();
        }
        
        [TestCase(6, new int[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,1,
            0,1,1,1
        })]
        [TestCase(0, new int[] {
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0
        })]
        [TestCase(10, new int[] {
            0,1,0,0,
            0,1,0,0,
            1,0,1,1,
            1,1,1,1
        })]
        public void AggregateHeight(int expected, int[] squares)
        {
            var heuristic = new YiyuanLeeHeuristic();
            var board = Build(4, 4, squares);

            var aggregateHeight = heuristic.AggregateHeight(board);

            Assert.AreEqual(expected, aggregateHeight);
        }

        [TestCase(1, new int[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,1,
            1,1,1,1
        })]
        [TestCase(0, new int[] {
            0,0,0,0,
            0,0,1,0,
            1,1,1,0,
            0,1,1,0
        })]
        [TestCase(2, new int[] {
            0,1,0,0,
            1,1,1,1,
            1,0,0,1,
            1,1,1,1
        })]
        public void CompleteLines(int expected, int[] squares)
        {
            var heuristic = new YiyuanLeeHeuristic();
            var board = Build(4, 4, squares);

            var completeLines = heuristic.CompleteLines(board);

            Assert.AreEqual(expected, completeLines);
        }

        [TestCase(0, new int[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,1,
            1,1,1,1
        })]
        [TestCase(3, new int[] {
            0,0,0,0,
            0,0,1,1,
            1,1,1,0,
            0,1,1,0
        })]
        [TestCase(1, new int[] {
            0,0,0,0,
            1,0,1,1,
            1,0,0,1,
            1,1,1,1
        })]
        public void Holes(int expected, int[] squares)
        {
            var heuristic = new YiyuanLeeHeuristic();
            var board = Build(4, 4, squares);

            var holes = heuristic.Holes(board);

            Assert.AreEqual(expected, holes);
        }

        [TestCase(5, new int[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,1,
            1,1,1,1
        })]
        [TestCase(0, new int[] {
            0,0,0,0,
            0,0,0,0,
            1,1,1,1,
            1,1,1,1
        })]
        [TestCase(7, new int[] {
            0,0,0,0,
            1,0,1,0,
            1,0,0,0,
            1,1,1,0
        })]
        public void Bumpiness(int expected, int[] squares)
        {
            var heuristic = new YiyuanLeeHeuristic();
            var board = Build(4, 4, squares);

            var bumpiness = heuristic.Bumpiness(board);

            Assert.AreEqual(expected, bumpiness);
        }

        private Board Build(int width, int height, int[] squares)
        {
            var board = new Board(width, height);
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    int index = (height - y - 1) * width + x;
                    if (squares[index] == 1)
                    {
                        board.Occupy(x, y);
                    }
                }
            }
            return board;
        }
    }
}
