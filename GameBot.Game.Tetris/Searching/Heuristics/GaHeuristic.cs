using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class GaHeuristic : BasicTetrisHeuristic
    {
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
            CalculateFast(gameState.Board);
            return
                -_heightWeigth * CalculatedAggregateHeight
                + _linesWeight * gameState.Lines
                - _holesWeight * CalculatedHoles
                - _bumpinessWeight * CalculatedBumpiness;
        }
    }
}
