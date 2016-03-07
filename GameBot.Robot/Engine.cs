using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Robot
{
    public class Engine : IEngine
    {
        private TimeSpan time;

        private readonly List<ICommand> commandQueue;
        private readonly IRenderer renderer;
        private readonly IAgent agent;
        private readonly Emulator emulator;

        public Engine(IRenderer renderer, IAgent agent, Emulator emulator)
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
            //Play();

            emulator.ExecuteFrame();
        }

        private void ReadKey()
        {
            var key = renderer.Key(1);
            if (key.HasValue)
            {
                if (key == 27) throw new TimeoutException(); // Escape
                if (key == 2490368) emulator.KeyTyped(Button.Up);
                if (key == 2621440) emulator.KeyTyped(Button.Down);
                if (key == 2424832) emulator.KeyTyped(Button.Left);
                if (key == 2555904) emulator.KeyTyped(Button.Right);
                if (key == 121) emulator.KeyTyped(Button.A);
                if (key == 120) emulator.KeyTyped(Button.B);
                if (key == 13) emulator.KeyTyped(Button.Start);
                if (key == 32) emulator.KeyTyped(Button.Select);
            }
        }

        private void Play()
        {
            ICommands commands = agent.Act(new Screenshot(emulator.Display, time));

            foreach (var command in commands)
            {
                commandQueue.Add(command);
            }

            var toExecute = commandQueue.Where(x => x.Timestamp <= time).ToList();
            foreach (var ex in toExecute)
            {
                emulator.KeyTyped(ex.Button);
            }
            
            commandQueue.RemoveAll(x => x.Timestamp <= time);
        }

        protected void Render()
        {
            renderer.Render(emulator.Display);
        }
    }
}
