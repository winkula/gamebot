using GameBot.Core;
using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisDecider : IDecider<TetrisGameState>
    {
        public ICommands Decide(TetrisGameState gameState, IContext<TetrisGameState> context)
        {
            throw new NotImplementedException();
        }
    }
}
