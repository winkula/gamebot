using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Renderers;
using System;
using System.Collections.Generic;

namespace GameBot.Robot.Engines
{
    public class InteractiveEngine : IEngine
    {
        private TimeSpan time;

        private readonly List<ICommand> commandQueue;
        private readonly IRenderer renderer;
        private readonly IAgent agent;
        private readonly Emulator emulator;

        public InteractiveEngine(IRenderer renderer, IAgent agent, Emulator emulator)
        {
            this.commandQueue = new List<ICommand>();
            this.time = TimeSpan.Zero;
            this.renderer = renderer;
            this.agent = agent;
            this.emulator = emulator;

            var loader = new RomLoader();
            var game = loader.Load("Roms/tetris.gb");
            this.emulator.Load(game);
        }

        public void Run()
        {
            renderer.OpenWindow("Game Bot");
            renderer.CreateImage(160, 144);

            Loop();

            renderer.End();
        }

        protected void Loop()
        {
            var start = DateTime.Now;
            while (true)
            {
                time = DateTime.Now - start;
                try
                {
                    Update();
                    Render();
                }
                catch (TimeoutException)
                {
                    break;
                }
            }
        }

        protected void Update()
        {
            ReadKey();

            emulator.ExecuteFrame();
        }

        private void ReadKey()
        {
            var key = renderer.Key(1);
            if (key.HasValue)
            {
                if (key == 27) throw new TimeoutException(); // Escape
                if (key == 2490368) emulator.Hit(Button.Up);
                if (key == 2621440) emulator.Hit(Button.Down);
                if (key == 2424832) emulator.Hit(Button.Left);
                if (key == 2555904) emulator.Hit(Button.Right);
                if (key == 121) emulator.Hit(Button.A);
                if (key == 120) emulator.Hit(Button.B);
                if (key == 13) emulator.Hit(Button.Start);
                if (key == 32) emulator.Hit(Button.Select);
            }
        }

        protected void Render()
        {
            renderer.Render(emulator.Display, "Game Bot");
        }
    }
}
