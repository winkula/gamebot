using System;
using Emgu.CV;

namespace GameBot.Core.Engines
{
    public abstract class BaseEngine : IEngine
    {
        protected readonly ICamera Camera;
        protected readonly IClock Clock;
        protected readonly IExecutor Executor;
        protected readonly IQuantizer Quantizer;
        protected readonly IAgent Agent;

        public bool Play { get; set; }

        protected BaseEngine(ICamera camera, IClock clock, IExecutor executor, IQuantizer quantizer, IAgent agent)
        {
            Camera = camera;
            Clock = clock;
            Executor = executor;
            Quantizer = quantizer;

            Agent = agent;
        }

        public void Initialize()
        {
            Clock.Start();
        }
        
        public abstract void Step(Action<Mat> showImage = null, Action<Mat> showProcessedImage = null);

        public void Reset()
        {
            Play = false;
            Agent.Reset();
        }
    }
}
