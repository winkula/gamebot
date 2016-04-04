using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;

namespace GameBot.Game.Tetris
{
    public class TetrisHeuristic : IHeuristic<TetrisGameState>
    {
        public double Score(TetrisGameState gameState)
        {
            return ScoreNearPerfect(gameState);
        }

        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        private double ScoreNearPerfect(TetrisGameState gameState)
        {
            var board = gameState.Board;

            var a = AggregateHeight(board);
            var c = gameState.Lines;
            var h = Holes(board);
            var b = Bumpiness(board);

            return -0.510066 * a + 0.760666 * c - 0.35663 * h - 0.184483 * b;
        }

        private double ScoreTest(TetrisGameState gameState)
        {
            var board = gameState.Board;
            var maxHeight = MaximumHeight(board);

            var m = maxHeight < 10 ? 0 : maxHeight - 10;
            var l = gameState.Lines == 0 ? 0 : gameState.Lines - 3;
            var h = Holes(board);          

            return -100 * m + 100 * l - 100 * h;
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
