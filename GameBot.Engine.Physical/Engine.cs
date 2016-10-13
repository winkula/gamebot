using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using System;

namespace GameBot.Engine.Physical
{
    public class Engine : IEngine
    {
        private readonly IConfig config;

        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IActuator actuator;
        private readonly IClock clock;

        public bool Play { get; set; }

        public Engine(IConfig config, ICamera camera, IQuantizer quantizer, IAgent agent, IActuator actuator, IClock clock)
        {
            this.config = config;

            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.actuator = actuator;
            this.clock = clock;
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

            callback(image, processed);

            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);

                // handle input to the agent which
                //  - extracts the game state
                //  - decides which commands to press
                //  - presses the buttons
                agent.Act(screenshot, actuator);
            }
        }
    }
}
