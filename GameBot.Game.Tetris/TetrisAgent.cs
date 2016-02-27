using GameBot.Core;
using GameBot.Core.Agents;

namespace GameBot.Game.Tetris
{
    public class TetrisAgent : AbstractAgent<TetrisGameState>
    {
        public TetrisAgent(IExtractor<TetrisGameState> extractor, IDecider<TetrisGameState> decider) : base(extractor, decider)
        {
        }
    }
}
