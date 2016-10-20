namespace GameBot.Game.Tetris.Extraction.Samplers
{
    /// <summary>
    /// Represents a generic implementation for a data collecting sampler.
    /// </summary>
    /// <typeparam name="TSample">Type of the sampled data.</typeparam>
    public interface ISampler<TSample>
    {
        void Sample(ProbabilisticResult<TSample> sample);

        int SampleCount { get; }

        bool IsComplete { get; }

        TSample Result { get; }
    }
}
