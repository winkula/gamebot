using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Ui;
using System;
using System.Collections.Generic;

namespace GameBot.Robot.Engines
{
    public class UiEngine : IEngine
    {
        private readonly IConfig config;

        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IActuator actuator;
        private readonly ITimeProvider timeProvider;
        private readonly IDebugger debugger;

        public UiEngine(IConfig config, ICamera camera, IQuantizer quantizer, IAgent agent, IActuator actuator, ITimeProvider timeProvider, IDebugger debugger)
        {
            this.config = config;

            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.actuator = actuator;
            this.timeProvider = timeProvider;
            this.debugger = debugger;
        }

        public void Run()
        {
            throw new NotSupportedException("Can only be called step by step.");
        }

        public void Initialize()
        {
            timeProvider.Start();
        }

        public void Step(bool play, Action<IImage, IImage> callback)
        {
            // get image as photo of the gameboy screen (input)
            IImage image = camera.Capture();

            // process image and get display data
            TimeSpan time = timeProvider.Time;
            IImage processed = quantizer.Quantize(image);

            processed = agent.Visualize(processed);

            callback(image, processed);

            if (play)
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
