using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class ContourHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public override double Score(GameState gameState)
        {
            CalculateFast(gameState.Board);
            return CalculatedHoles;
        }
    }
}
