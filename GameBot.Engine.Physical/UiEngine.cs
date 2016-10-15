using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using System;

namespace GameBot.Engine.Physical
{
    public class UiEngine : IEngine
    {
        private readonly IConfig config;

        private readonly IActuator actuator;
        private readonly ICamera camera;
        private readonly IClock clock;
        private readonly IExecutor executor;
        private readonly IQuantizer quantizer;

        private readonly IAgent agent;

        public bool Play { get; set; }

        public UiEngine( ICamera camera, IConfig config, IExecutor executor, IQuantizer quantizer, IAgent agent, IActuator actuator, IClock clock)
        {
            this.config = config;

            this.camera = camera;
            this.clock = clock;
            this.executor = executor;
            this.quantizer = quantizer;
            this.agent = agent;
            this.actuator = actuator;
        }

        public void Initialize()
        {
            clock.Start();
        }

        public void Step(Action<IImage, IImage> callback = null)
        {
            // get image as photo of the gameboy screen (input)
            IImage image = camera.Capture();

            // process image and get display data
            TimeSpan time = clock.Time;
            IImage processed = quantizer.Quantize(image);
            
            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);

                // handle input to the agent which
                //  - extracts the game state
                //  - decides which commands to press
                //  - presses the buttons
                agent.Act(screenshot, executor);
                processed = agent.Visualize(processed);
            }

            callback?.Invoke(image, processed);
        }

        public void Reset()
        {
            Play = false;
            agent.Reset();
        }
    }
}
