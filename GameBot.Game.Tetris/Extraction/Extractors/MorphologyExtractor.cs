using GameBot.Core;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class MorphologyExtractor : BaseExtractor
    {
        public MorphologyExtractor(IConfig config) : base(config, new MorphologyMatcher())
        {
        }
    }
}
