using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;

namespace GameBot.Game.Tetris
{
    public class TetrisHeuristic : IHeuristic<TetrisGameState>
    {
        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public double Score(TetrisGameState gameState)
        {
            var board = gameState.Board;

            var a = AggregateHeight(board);
            var c = gameState.Lines;
            var h = Holes(board);
            var b = Bumpiness(board);
            
            return -0.510066 * a + 0.760666 * c - 0.35663 * h - 0.184483 * b;
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
