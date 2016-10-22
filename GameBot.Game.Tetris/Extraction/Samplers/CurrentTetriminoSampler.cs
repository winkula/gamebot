using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Samplers
{
    public class CurrentTetriminoSampler : ISampler<Piece>
    {
        private readonly IList<ProbabilisticResult<Piece>> _samples;

        private readonly int _numSamples;
        private readonly int _numSamplesMajority;

        public CurrentTetriminoSampler(int numSamples)
        {
            if (numSamples < 1) throw new ArgumentException("numSamples must be >= 1");
            if (numSamples % 2 == 0) throw new ArgumentException("numSamples must be odd");

            _samples = new List<ProbabilisticResult<Piece>>();

            _numSamples = numSamples;
            _numSamplesMajority = 1 + numSamples / 2;
        }

        public void Sample(ProbabilisticResult<Piece> sample)
        {
            if (IsComplete) throw new ArgumentException("sample limit exceeded");

            _samples.Add(sample);
        }

        public int SampleCount => _samples.Count;

        public bool IsComplete => SampleCount >= _numSamples || MajorityCount >= _numSamplesMajority;

        private int MajorityCount
        {
            get
            {
                if (SampleCount > 0)
                {
                    return _samples
                        .GroupBy(x => x.Result.Tetrimino, y => y.Probability)
                        .Select(x => new { Piece = x.Key, Number = x.Count() })
                        .Max(x => x.Number);
                }
                return 0;
            }
        }

        public Piece Result
        {
            get
            {
                // group by tetrimino, orientation and x coordinate to make evaluation Y invariant
                // we then take the last matching sample and take its y coordinate
                var samplesOrderedGrouped = _samples
                    .GroupBy(x => x.Result.Tetrimino, y => y.Probability)
                    .Select(x => new { Tetromino = x.Key, Number = x.Count(), ProbabilityAvg = x.Average() })
                    .OrderByDescending(x => x.Number)
                    .ThenByDescending(x => x.ProbabilityAvg)
                    .ToList();
                var tetromino = samplesOrderedGrouped.First().Tetromino;
                var yCoordinate = _samples
                    .Reverse()
                    .First(x => x.Result.Tetrimino == tetromino)
                    .Result.Y;

                return new Piece(tetromino, 0, 0, yCoordinate);
            }
        }
    }
}