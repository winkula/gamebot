using System;

namespace GameBot.Core.Data
{
    public abstract class AbstractGameState : IGameState
    {
        public TimeSpan Timestamp { get; set; }
    }
}
