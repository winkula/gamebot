using GameBot.Core.Data;

namespace GameBot.Core.Searching
{
    public interface IHeuristic
    {
    }

    public interface IHeuristic<T> : IHeuristic where T : class, IGameState
    {
        double Score(T gameState);
    }
}
