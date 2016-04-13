using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Robot.Engines
{
    public class FastEngine : IEngine
    {
        private readonly ISolver<TetrisGameState> solver;
        private readonly TetrisSimulator emulator;

        public FastEngine(ISolver<TetrisGameState> solver, TetrisSimulator emulator)
        {
            this.solver = solver;
            this.emulator = emulator;
        }

        public void Run()
        {
            // TODO: remove initialization
            IEnumerable<ICommand> commands = solver.Solve(emulator.GameState);

            Loop();
        }

        protected void Loop()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (i % 50 == 0)
                {
                    Debug.WriteLine("Play round " + i + "...");
                }

                if (emulator.GameState.IsEnd)
                {
                    Debug.WriteLine("----- Lost! -----");
                    Debug.WriteLine("Played rounds: " + i);
                    Debug.WriteLine("Lines: " + emulator.GameState.Lines);
                    Debug.WriteLine("Score: " + emulator.GameState.Score);
                    Debug.WriteLine("Level: " + emulator.GameState.Level);
                    Debug.WriteLine("Elapsed time in ms: " + stopwatch.ElapsedMilliseconds);

                    return;
                }

                Update();
                //Render();
            }
            stopwatch.Stop();
        }

        protected void Update()
        {
            IEnumerable<ICommand> commands = solver.Solve(emulator.GameState);
            if (commands.Any())
            {
                foreach (var command in commands)
                {
                    if (command.Button != Button.Down)
                    {
                        emulator.Simulate(command);
                        //Debug.WriteLine(command.Button);
                        //Render();
                    }
                }
                // drop
                emulator.Simulate(new HitCommand(Button.Down, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
                //Debug.WriteLine("Drop");
                //Render();
            }
            else
            {
                emulator.Simulate(new HitCommand(Button.Down));
                Render();
            }
        }

        protected void Render()
        {
            //Debug.WriteLine(emulator.GameState);
        }
    }
}
