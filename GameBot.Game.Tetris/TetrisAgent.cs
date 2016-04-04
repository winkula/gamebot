using GameBot.Core;
using GameBot.Core.Agents;

namespace GameBot.Game.Tetris
{
    public class TetrisAgent : AbstractAgent<TetrisGameState>
    {
        public TetrisAgent(IExtractor<TetrisGameState> extractor, ISolver<TetrisGameState> solver) : base(extractor, solver)
        {
        }
    }
}
