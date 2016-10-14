using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris.Data;
using NLog;
using System;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisStartState : ITetrisState
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private TetrisAgent agent;

        private readonly int startLevel;

        public TetrisStartState(TetrisAgent agent, int startLevel)
        {
            this.agent = agent;

            this.startLevel = startLevel;
        }

        public void Act()
        {
            // wait before start
            var random = new Random();
            var randomTime = 2.1 + 0.5 * random.NextDouble();
            if (agent.Clock.Time >= TimeSpan.FromSeconds(randomTime))
            {
                var commands = Initialize();

                foreach (var command in commands)
                {
                    command.Execute(agent.Actuator);
                }

                // init game state
                agent.GameState = new GameState();
                agent.GameState.StartLevel = startLevel;

                logger.Info("> Game started. Initialization sequence executed.");
                agent.SetState(new TetrisAnalyzeState(agent, null));
            }
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
            SelectLevel(commands, startLevel);
        }

        private void SelectLevel(CommandCollection commands, int startLevel)
        {
            if (startLevel < 0 || startLevel > 9)
                throw new ArgumentException("startLevel must be between 0 and 9 (inclusive)");

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
