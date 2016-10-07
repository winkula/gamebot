using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public interface IHeuristic
    {
        double Score(GameState gameState);
    }
}
