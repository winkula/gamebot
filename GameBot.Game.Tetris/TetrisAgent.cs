using GameBot.Core;
using GameBot.Core.Agents;

namespace GameBot.Game.Tetris
{
    public class TetrisAgent : AbstractAgent<TetrisGameStateFull>
    {
        public TetrisAgent(IExtractor<TetrisGameStateFull> extractor, IDecider<TetrisGameStateFull> decider) : base(extractor, decider)
        {
        }
    }
}
