using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Renderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace GameBot.Robot.Engines
{
    public class Engine : IEngine
    {
        private readonly IConfig config;

        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IExecutor executor;
        private readonly ITimeProvider timeProvider;

        public Engine(IConfig config, ICamera camera, IQuantizer quantizer, IAgent agent, IExecutor executor, ITimeProvider timeProvider)
        {
            this.config = config;

            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.executor = executor;
            this.timeProvider = timeProvider;
        }

        public void Run()
        {
            timeProvider.Start();

            Loop();

            //renderer.End();
        }

        protected void Loop()
        {
            while (true)
            {
                // get image as photo of the gameboy screen (input)
                IImage image = camera.Capture();
                Render(image);

                // process image and get display data
                TimeSpan time = timeProvider.Time;
                IImage processed = quantizer.Quantize(image);
                IScreenshot screenshot = new EmguScreenshot(processed, time);

                // handle input to the agent which
                //  - extracts the game state
                //  - decides which commands to press
                IEnumerable<ICommand> commands = agent.Act(screenshot);

                // give commands to command controller (output)
                executor.Execute(commands);
            }
        }
        
        protected void Render(IImage image)
        {
            //renderer.Render(image, "Image_Captured");
        }

        public void Configure(string key, object value)
        {
        }

        public EngineResult Step()
        {
            throw new NotImplementedException();
        }
    }
}
