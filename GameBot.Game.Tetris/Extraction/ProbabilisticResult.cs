using System;

namespace GameBot.Game.Tetris.Extraction
{
    public class ProbabilisticResult<TResult>
    {
        public TResult Result { get; }
        public double Probability { get; }
        
        public ProbabilisticResult(TResult result, double probability = 0.5)
        {
            if (probability < 0 || probability > 1.0)
                throw new ArgumentException("probability must be between 0.0 and 1.0");

            Result = result;
            Probability = probability;
        }

        public bool IsAccepted(double lowerThreshold)
        {
            if (lowerThreshold < 0.0 || lowerThreshold > 1.0)
                throw new ArgumentException("lowerThreshold must be between 0.0 and 1.0 (inclusive)");

            return Probability >= lowerThreshold;
        }
        
        public bool IsRejected(double lowerThreshold)
        {
            if (lowerThreshold < 0.0 || lowerThreshold > 1.0)
                throw new ArgumentException("lowerThreshold must be between 0.0 and 1.0 (inclusive)");

            return Probability < lowerThreshold;
        }
    }
}
