using System;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Samplers;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction.Samplers
{
    [TestFixture]
    public class MultiplayerPenaltyLinesHolePositionSamplerTests
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(17)]
        public void Constructor(int numSamples)
        {
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            Assert.NotNull(sampler);
        }

        [TestCase(-10)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(8)]
        public void ConstructorFails(int numSamples)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

                Assert.NotNull(sampler);
            });
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Sample(int numSamples)
        {
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            for (int i = 0; i < numSamples; i++)
            {
                var sample = new ProbabilisticResult<int>(i, 0.5);
                sampler.Sample(sample);
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void SampleFails(int numSamples)
        {
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            for (int i = 0; i < numSamples; i++)
            {
                var sample = new ProbabilisticResult<int>(i, 0.5);
                sampler.Sample(sample);
            }

            Assert.Throws<ArgumentException>(() =>
            {
                sampler.Sample(new ProbabilisticResult<int>(9, 0.5));
            });
        }

        [Test]
        public void CompleteByNumber()
        {
            int numSamples = 3;
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            sampler.Sample(new ProbabilisticResult<int>(7, 0.3));
            sampler.Sample(new ProbabilisticResult<int>(2, 0.5));
            sampler.Sample(new ProbabilisticResult<int>(7, 0.2));

            var complete = sampler.IsComplete;

            Assert.True(complete);
        }

        [Test]
        public void CompleteByMajority()
        {
            int numSamples = 3;
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            sampler.Sample(new ProbabilisticResult<int>(6, 0.3));
            sampler.Sample(new ProbabilisticResult<int>(6, 0.5));

            var complete = sampler.IsComplete;

            Assert.True(complete);
        }

        [Test]
        public void Incomplete()
        {
            int numSamples = 3;
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            sampler.Sample(new ProbabilisticResult<int>(7, 0.3));
            sampler.Sample(new ProbabilisticResult<int>(2, 0.5));

            var complete = sampler.IsComplete;

            Assert.False(complete);
        }

        [Test]
        public void CaseMixed()
        {
            int numSamples = 3;
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            sampler.Sample(new ProbabilisticResult<int>(6, 0.4));
            Assert.False(sampler.IsComplete);

            sampler.Sample(new ProbabilisticResult<int>(5, 0.5));
            Assert.False(sampler.IsComplete);

            sampler.Sample(new ProbabilisticResult<int>(4, 0.3));
            Assert.True(sampler.IsComplete);

            var result = sampler.Result;

            Assert.AreEqual(5, result);
        }

        [Test]
        public void CaseMajority()
        {
            int numSamples = 3;
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            sampler.Sample(new ProbabilisticResult<int>(7, 0.3));
            Assert.False(sampler.IsComplete);

            sampler.Sample(new ProbabilisticResult<int>(3, 0.8));
            Assert.False(sampler.IsComplete);

            sampler.Sample(new ProbabilisticResult<int>(7, 0.3));
            Assert.True(sampler.IsComplete);

            var result = sampler.Result;

            Assert.AreEqual(7, result);
        }

        [Test]
        public void CaseEarlyMajority()
        {
            int numSamples = 3;
            var sampler = new MultiplayerPenaltyLinesHolePositionSampler(numSamples);

            sampler.Sample(new ProbabilisticResult<int>(2, 0.3));
            Assert.False(sampler.IsComplete);

            sampler.Sample(new ProbabilisticResult<int>(2, 0.8));
            Assert.True(sampler.IsComplete);

            var result = sampler.Result;

            Assert.AreEqual(2, result);
        }
    }
}
