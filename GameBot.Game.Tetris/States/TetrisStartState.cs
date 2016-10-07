using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris.Agents;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameBot.Game.Tetris.States
{
    public class TetrisStartState : ITetrisState
    {
        private TetrisAgent agent;

        private readonly int StartLevel;

        public TetrisStartState(int startLevel)
        {
            StartLevel = startLevel;
        }

        public void Act(TetrisAgent agent)
        {
            this.agent = agent;

            var commands = Initialize();

            foreach (var command in commands)
            {
                command.Execute(agent.Actuator);
            }

            Debug.WriteLine("> Game started. Initialization sequence executed.");
            agent.SetState(new TetrisAnalyzeState(null));
        }

        private IEnumerable<ICommand> Initialize()
        {
            var commands = new CommandCollection();
            Start(commands);
            return commands;
        }

        private void Start(CommandCollection commands)
        {
            // skip credits
            double waitingTime = 2.2 + new Random().NextDouble();

            // start 1 player mode
            commands.Hit(Button.Start, waitingTime);

            // choose a-type
            commands.HitDelta(Button.A);

            // choose music
            commands.HitDelta(Button.Right);
            commands.HitDelta(Button.Down);
            commands.HitDelta(Button.A);

            // select level
            SelectLevel(commands, StartLevel);
        }

        private void SelectLevel(CommandCollection commands, int startLevel)
        {
            if (startLevel >= 5)
            {
                commands.HitDelta(Button.Down);
            }
            for (int i = 0; i < (startLevel % 5); i++)
            {
                commands.HitDelta(Button.Right);
            }
            commands.HitDelta(Button.A);
        }
    }
}
