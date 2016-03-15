using System;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisExtractor : IExtractor<TetrisGameStateFull>
    {
        public TetrisGameStateFull Extract(IScreenshot screenshot, IContext<TetrisGameStateFull> context)
        {
            var gameState = new TetrisGameStateFull();

            return gameState;
        }
    }
}
