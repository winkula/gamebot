using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Renderers;
using System;
using System.Diagnostics;

namespace GameBot.Robot.Engines
{
    public class Engine : IEngine
    {
        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IExecutor executor;

        private readonly ITimeProvider timeProvider;

        private readonly IRenderer renderer;

        private readonly Emulator emulator;
        
        public Engine(ICamera camera, IQuantizer quantizer, IAgent agent, IExecutor executor, ITimeProvider timeProvider, IRenderer renderer, Emulator emulator)
        {
            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.executor = executor;
            this.timeProvider = timeProvider;
            executor.TimeProvider = timeProvider;

            this.renderer = renderer;

            this.emulator = emulator;

            var loader = new RomLoader();
            var game = loader.Load("Roms/tetris.gb");
            this.emulator.Load(game);
        }

        public void Run()
        {
            renderer.OpenWindow("Game Bot");
            renderer.CreateImage(160, 144);

            timeProvider.Start();

            Loop();

            renderer.End();
        }

        protected void Loop()
        {
            while (!IsEscape)
            {
                Update();
                Render();
                Debug.Write(timeProvider.Time + "\n");
            }
        }

        protected void Update()
        {
            // get image as photo of the gameboy screen (input)
            IImage image = camera.Capture();

            // process image and get display data
            TimeSpan time = timeProvider.Time;
            IScreenshot screenshot = quantizer.Quantize(image, time);

            // handle input to the agent which
            //  - extracts the game state
            //  - decides which commands to press
            ICommands commands = agent.Act(screenshot);
            
            // give commands to command controller (output)
            executor.Execute(commands);
            
            /*
            foreach (var command in commands)
            {
                // give command to command controller (output)
                executor.Execute(command, time);

                //Render();
            }*/
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

        protected void Render()
        {
            renderer.Render(emulator.Display);
        }
    }
}
