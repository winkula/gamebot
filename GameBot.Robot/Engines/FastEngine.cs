using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris;
using System;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Robot.Engines
{
    public class FastEngine : IEngine
    {
        private readonly ISolver<TetrisGameState> solver;
        private readonly TetrisEmulator emulator;

        public FastEngine(ISolver<TetrisGameState> solver, TetrisEmulator emulator)
        {
            this.solver = solver;
            this.emulator = emulator;
        }

        public void Run()
        {
            // TODO: remove initialization
            ICommands commands = solver.Solve(emulator.GameState);

            Loop();
        }

        protected void Loop()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int games = 10000;
            for (int i = 0; i < games; i++)
            {
                Debug.WriteLine("Play round " + i + "...");

                if (emulator.GameState.IsEnd)
                {
                    Debug.WriteLine("Lost!");
                    return;
                }

                Update();
                //Render();
            }
            stopwatch.Stop();
            Debug.WriteLine("Successfully played " + games + " rounds!");
            Debug.WriteLine("Elapsed time in ms: " + stopwatch.ElapsedMilliseconds);
        }

        protected void Update()
        {
            ICommands commands = solver.Solve(emulator.GameState);
            if (commands.Any())
            {
                foreach (var command in commands)
                {
                    if (command.Button != Button.Down)
                    {
                        emulator.Execute(command);
                        //Debug.WriteLine(command.Button);
                        //Render();
                    }
                }
                // drop
                emulator.Execute(new Command(Button.Down, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
                //Debug.WriteLine("Drop");
                //Render();
            }
            else
            {
                emulator.Execute(new Command(Button.Down));
                Render();
            }
        }

        protected void Render()
        {
            Debug.WriteLine(emulator.GameState);
        }
    }
}
