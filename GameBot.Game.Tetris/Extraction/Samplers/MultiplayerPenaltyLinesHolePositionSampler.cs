using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Extraction.Samplers
{
    public class MultiplayerPenaltyLinesHolePositionSampler : ISampler<int>
    {
        private readonly IList<ProbabilisticResult<int>> _samples;
        
        private readonly int _numSamples;
        private readonly int _numSamplesMajority;

        public MultiplayerPenaltyLinesHolePositionSampler(int numSamples)
        {
            if (numSamples < 1) throw new ArgumentException("numSamples must be >= 1");
            if (numSamples % 2 == 0) throw new ArgumentException("numSamples must be odd");

            _samples = new List<ProbabilisticResult<int>>();
            
            _numSamples = numSamples;
            _numSamplesMajority = 1 + numSamples / 2;
        }

        public void Sample(ProbabilisticResult<int> sample)
        {
            if (IsComplete) throw new ArgumentException("sample limit exceeded");

            _samples.Add(sample);
        }

        public int SampleCount => _samples.Count;

        public bool IsComplete =>
            SampleCount >= _numSamples ||
            MajorityCount >= _numSamplesMajority;

        private int MajorityCount
        {
            get
            {
                if (SampleCount > 0)
                {
                    return _samples
                        .GroupBy(x => x.Result, y => y.Result)
                        .Select(x => new { Piece = x.Key, Number = x.Count() })
                        .Max(x => x.Number);
                }
                return 0;
            }
        }

        public int Result
        {
            get
            {
                return _samples
                    .GroupBy(x => x.Result, y => y.Result)
                    .Select(x => new { Value = x.Key, Number = x.Count() })
                    .OrderByDescending(x => x.Number)
                    .First()
                    .Value;
            }
        }
    }
}