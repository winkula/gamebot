using System;

namespace GameBot.Game.Tetris.Extraction
{
    public class ProbabilisticResult<TResult>
    {
        public TResult Result { get; }
        public double Probability { get; }

        public ProbabilisticResult(TResult result, double probability)
        {
            if (probability < 0 || probability > 1.0)
                throw new ArgumentException("probability must be between 0.0 and 1.0");

            Result = result;
            Probability = probability;
        }
    }
}
