using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Renderers;
using System;
using System.Drawing;

namespace GameBot.Robot.Engines
{
    public class BlindEngine : IEngine
    {
        private TimeSpan time;

        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IExecutor executor;

        private readonly IRenderer renderer;

        private readonly Emulator emulator;

        public BlindEngine(ICamera camera, IQuantizer quantizer, IAgent agent, IExecutor executor, IRenderer renderer, Emulator emulator)
        {
            this.time = TimeSpan.Zero;

            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.executor = executor;

            this.renderer = renderer;

            this.emulator = emulator;

            var loader = new RomLoader();
            var game = loader.Load("Roms/tetris.gb");
            this.emulator.Load(game);
        }

        public void Run()
        {
            //renderer.OpenWindow("Game Bot");
            //renderer.CreateImage(160, 144);

            Loop();

            //renderer.End();
        }

        protected void Loop()
        {
            var start = DateTime.Now;
            while (!IsEscape)
            {
                time = DateTime.Now - start;

                Update();
                //Render();
            }
        }

        protected void Update()
        {
            // get image as photo of the gameboy screen (input)
            Image image = camera.Capture();

            // process image and get display data
            IScreenshot screenshot = quantizer.Quantize(image, time);

            // handle input to the agent which
            //  - extracts the game state
            //  - decides which commands to press
            ICommands commands = agent.Act(screenshot);
            
            // give commands to command controller (output)
            executor.Execute(commands, time);
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
