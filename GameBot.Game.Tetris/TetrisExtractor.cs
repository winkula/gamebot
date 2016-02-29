using System;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisExtractor : IExtractor<TetrisGameState>
    {
        public TetrisGameState Extract(IScreenshot screenshot, IContext<TetrisGameState> context)
        {
            throw new NotImplementedException();
        }
    }
}
