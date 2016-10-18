using System;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class YiyuanLeeHeuristic : BasicTetrisHeuristic
    {
        private int _aggregateHeight;
        private int _holes;
        private int _bumpiness;

        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public override double Score(GameState gameState)
        {
            var board = gameState.Board;
            CalculateFast(board);

            var a = _aggregateHeight;
            var c = gameState.Lines;
            var h = _holes;
            var b = _bumpiness;
            
            return -0.510066 * a + 0.760666 * c - 0.35663 * h - 0.184483 * b;
        }

        private void CalculateFast(Board board)
        {
            int aggregateHeight = 0;

            int holes = 0;

            int bumpiness = 0;
            int? lastHeight = null;

            for (int x = 0; x < board.Width; x++)
            {
                aggregateHeight += board.ColumnHeight(x);

                holes += board.ColumnHoles(x);

                int height = board.ColumnHeight(x);
                if (lastHeight.HasValue)
                {
                    bumpiness += Math.Abs(lastHeight.Value - height);
                }
                lastHeight = height;
            }

            _aggregateHeight = aggregateHeight;
            _holes = holes;
            _bumpiness = bumpiness;
        }
    }
}
