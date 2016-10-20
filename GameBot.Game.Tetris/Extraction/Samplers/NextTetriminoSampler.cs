using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Samplers
{
    public class NextTetriminoSampler : ISampler<Tetrimino>
    {
        private readonly IList<ProbabilisticResult<Tetrimino>> _samples;

        private readonly int _numSamples;
        private readonly int _numSamplesMajority;
        
        public NextTetriminoSampler(int numSamples)
        {
            if (numSamples < 1) throw new ArgumentException("numSamples must be >= 1");
            if (numSamples % 2 == 0) throw new ArgumentException("numSamples must be odd");

            _samples = new List<ProbabilisticResult<Tetrimino>>();

            _numSamples = numSamples;
            _numSamplesMajority = 1 + numSamples / 2;
        }

        public void Sample(ProbabilisticResult<Tetrimino> sample)
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
                        .GroupBy(x => x.Result, y => y.Probability)
                        .Select(x => new { Piece = x.Key, Number = x.Count() })
                        .Max(x => x.Number);
                }
                return 0;
            }
        }

        public Tetrimino Result
        {
            get
            {
                var samplesOrderedGrouped = _samples
                    .GroupBy(x => x.Result, y => y.Probability)
                    .Select(x => new { NextPiece = x.Key, Number = x.Count(), ProbabilityAvg = x.Average() })
                    .OrderByDescending(x => x.Number)
                    .ThenByDescending(x => x.ProbabilityAvg)
                    .ToList();
                var nextPiece = samplesOrderedGrouped.First().NextPiece;
                return nextPiece;
            }
        }
    }
}