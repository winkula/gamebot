using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;
using System;

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
                // handle start menu
                //StartFromMenu(agent.Actuator);
                StartFromGameOver(agent.Actuator);

                // init game state
                agent.GameState = new GameState();
                agent.GameState.StartLevel = startLevel;

                logger.Info("> Game started. Initialization sequence executed.");
                agent.SetState(new TetrisAnalyzeState(agent, null));
            }
        }
        
        private void StartFromMenu(IActuator actuator)
        {
            // start 1 player mode
            actuator.Hit(Button.Start);

            // choose a-type
            actuator.Hit(Button.A);

            // choose music
            actuator.Hit(Button.Right);
            actuator.Hit(Button.Down);
            actuator.Hit(Button.A);

            // select level
            SelectLevel(actuator, startLevel);
        }

        private void StartFromGameOver(IActuator actuator)
        {
            actuator.Hit(Button.Start);
            actuator.Hit(Button.Start);
        }

        private void SelectLevel(IActuator actuator, int startLevel)
        {
            if (startLevel < 0 || startLevel > 9)
                throw new ArgumentException("startLevel must be between 0 and 9 (inclusive)");

            if (startLevel >= 5)
            {
                actuator.Hit(Button.Down);
            }
            for (int i = 0; i < (startLevel % 5); i++)
            {
                actuator.Hit(Button.Right);
            }
            actuator.Hit(Button.A);
        }
    }
}
