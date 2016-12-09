using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class YiyuanLeeHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public override double Score(GameState gameState)
        {
            CalculateFast(gameState.Board);
            return
                -0.510066 * CalculatedAggregateHeight
                + 0.760666 * gameState.Lines
                - 0.35663 * CalculatedHoles
                - 0.184483 * CalculatedBumpiness;
        }
    }
}
