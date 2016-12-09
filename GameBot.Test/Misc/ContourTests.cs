using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using NUnit.Framework;

namespace GameBot.Test.Misc
{
    [TestFixture]
    public class ContourTests
    {
        private const int _contourMinMaxDelta = 3;
        private const int _contourWidth = 6;
        private readonly ContourHeuristic _heuristic = new ContourHeuristic();

        [Test]
        public void GenerateAllContours()
        {
            ContourRecursive(new Stack<int>(), _contourWidth);
        }

        private void ContourRecursive(Stack<int> deltas, int level)
        {
            if (level == 0)
            {
                try
                {
                    var value = Evaluate(deltas);
                    //Debug.WriteLine(value);
                }
                catch (ArgumentException)
                {
                    // ignore invalids
                }
                return;
            }

            for (int i = -_contourMinMaxDelta; i <= _contourMinMaxDelta; i++)
            {
                deltas.Push(i);
                ContourRecursive(deltas, level - 1);
                deltas.Pop();
            }
        }

        private double Evaluate(Stack<int> deltas)
        {
            var deltasArray = deltas.ToArray();
            var begin = GetBegin(deltas);
            var board = new Board();

            var height = begin;
            for (int x = 0; x < board.Width; x++)
            {
                if (height > 0)
                {
                    board.FillColumn(x, height);
                }
                if (x < _contourWidth)
                {
                    height += deltasArray[x];
                }
            }

            var summedHoles = 0;
            foreach (var tetrimino in Tetriminos.All)
            {
                var bestScore = 255;
                var root = new Node(new GameState(new Board(board), tetrimino));

                foreach (var successor in root.GetSuccessors())
                {
                    var score = _heuristic.Holes(successor.GameState.Board);
                    if (score < bestScore)
                    {
                        bestScore = score;
                    }
                }

                summedHoles += bestScore;
            }

            return Math.Min(summedHoles, 255);
        }

        private int GetBegin(Stack<int> deltas)
        {
            int min = 0;
            int max = 0;
            int start = 0;
            int now = start;
            
            foreach (var delta in deltas)
            {
                now += delta;
                min = Math.Min(now, min);
                max = Math.Max(now, max);
            }

            return -min;
        }
    }
}
