using System.Collections.Generic;
using System.Linq;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class MorphologyExtractor : BaseExtractor
    {
        public MorphologyExtractor(IConfig config) : base(config, new MorphologyMatcher())
        {
        }

        /*
        public override Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            Tetrimino? candidate = null;
            int candidates = 0;

            foreach (var tetrimino in Tetriminos.All)
            {
                var probability = Matcher.GetProbabilityNextPiece(screenshot, tetrimino);
                if (probability > ThresholdNextPiece)
                {
                    candidate = tetrimino;
                    candidates++;
                }
            }

            if (candidates == 1)
            {
                // we only allow one candidate
                return candidate;
            }

            return null;
        }
        */
    }
}
