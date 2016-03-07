using GameBot.Core.Data;
using System.Collections.Generic;

namespace GameBot.Core
{
    public class Context<T> : IContext<T> where T : class, IGameState
    {
        public ICollection<T> GameStates { get; private set; }

        public Context()
        {
            GameStates = new List<T>();
        }

        public void Add(T gameState)
        {
            GameStates.Add(gameState);
        }
    }
}
