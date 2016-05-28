using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Renderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameBot.Robot.Engines
{
    public class FancyEngine : IEngine
    {
        private readonly IConfig config;

        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IExecutor executor;
        private readonly ITimeProvider timeProvider;

        private readonly IRenderer renderer;

        public FancyEngine(IConfig config, ICamera camera, IQuantizer quantizer, IAgent agent, IExecutor executor, ITimeProvider timeProvider, IRenderer renderer)
        {
            this.config = config;

            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.executor = executor;
            this.timeProvider = timeProvider;

            this.renderer = renderer;
        }

        public void Run()
        {
            timeProvider.Start();

            Loop();

            renderer.End();
        }

        protected void Loop()
        {
            while (!IsEscape)
            {
                // get image as photo of the gameboy screen (input)
                IImage image = camera.Capture();
                Render(image);

                // process image and get display data
                TimeSpan time = timeProvider.Time;
                IScreenshot screenshot = quantizer.Quantize(image, time);

                // handle input to the agent which
                //  - extracts the game state
                //  - decides which commands to press
                IEnumerable<ICommand> commands = agent.Act(screenshot);

                // give commands to command controller (output)
                executor.Execute(commands);
            }
        }

        private bool IsEscape
        {
            get
            {
                var key = renderer.Key(1);
                if (key.HasValue)
                {
                    if (key == 27) return true; // Escape
                }
                return false;
            }
        }

        protected void Render(IImage image)
        {
            renderer.Render(image, "Image_Captured");
        }

        public void Configure(string key, object value)
        {
        }
    }
}
