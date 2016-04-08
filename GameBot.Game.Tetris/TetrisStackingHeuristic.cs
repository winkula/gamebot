using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;

namespace GameBot.Game.Tetris
{
    public class TetrisStackingHeuristic : IHeuristic<TetrisGameState>
    {
        public double Score(TetrisGameState gameState)
        {
            var board = gameState.Board;

            var heightLastCol = board.ColumnHeight(9);
            var a = Math.Max(45, AggregateHeight(board)) - 45;
            var c = gameState.Lines;
            var h = Holes(board);
            var b = Bumpiness(board);

            return -2 * a + 0.3 * c - 2 * h - 0.184483 * b - 2 * heightLastCol;
        }

        public int MaximumHeight(Board board)
        {
            return board.MaximumHeight;
        }

        // top down
        // optimized
        public int AggregateHeight(Board board)
        {
            int aggregateHeight = 0;
            for (int x = 0; x < board.Width; x++)
            {
                aggregateHeight += board.ColumnHeight(x);
            }
            return aggregateHeight;
        }
        
        // bottom up
        // optimized
        public int CompleteLines(Board board)
        {
            return board.CompletedLines;
        }

        // bottom up
        // optimized
        public int Holes(Board board)
        {
            int holes = 0;
            for (int x = 0; x < board.Width; x++)
            {
                holes += board.ColumnHoles(x);
            }
            return holes;
        }

        // top down
        // optimized
        public int Bumpiness(Board board)
        {
            int bumpiness = 0;
            int? lastHeight = null;
            for (int x = 0; x < board.Width; x++)
            {
                int height = board.ColumnHeight(x);
                if (lastHeight.HasValue)
                {
                    bumpiness += Math.Abs(lastHeight.Value - height);
                }
                lastHeight = height;
            }
            return bumpiness;
        }
    }
}
