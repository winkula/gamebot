using System;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class GaHeuristic : BasicTetrisHeuristic
    {
        private int _aggregateHeight;
        private int _holes;
        private int _bumpiness;

        private readonly double _heightWeigth;
        private readonly double _linesWeight;
        private readonly double _holesWeight;
        private readonly double _bumpinessWeight;

        public GaHeuristic(double heightWeigth, double linesWeight, double holesWeight, double bumpinessWeight)
        {
            _heightWeigth = heightWeigth;
            _linesWeight = linesWeight;
            _holesWeight = holesWeight;
            _bumpinessWeight = bumpinessWeight;
        }

        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public override double Score(GameState gameState)
        {
            var board = gameState.Board;
            CalculateFast(board);

            var a = _aggregateHeight;
            var c = gameState.Lines;
            var h = _holes;
            var b = _bumpiness;

            return
                -_heightWeigth * a
                + _linesWeight * c
                - _holesWeight * h
                - _bumpinessWeight * b;
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
