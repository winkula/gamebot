using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;
using System;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisStartState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Random _random = new Random();

        private readonly TetrisAgent _agent;

        private readonly bool _startFromGameover;
        private readonly int _startLevel;

        public TetrisStartState(TetrisAgent agent, int startLevel, bool startFromGameOver)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            _agent = agent;
            _startFromGameover = startFromGameOver;
            _startLevel = startLevel;
        }

        public void Extract()
        {
            // do nothing
        }
        
        public void Play()
        {
            if (IsStartScreenVisble())
            {
                if (_startFromGameover)
                {
                    _logger.Info("Game started from game over screen");

                    // handle start menu
                    StartFromGameOver(_agent.Executor);
                }
                else
                {
                    _logger.Info("Game started from start menu");

                    // restart from game over screen (good for testing multiple games)
                    StartFromMenu(_agent.Executor);
                }
                
                // init game state
                _agent.GameState = new GameState { StartLevel = _startLevel };
                SetStateAnalyze();
            }
        }
        
        private bool IsStartScreenVisble()
        {
            var randomTime = 2.5 + _random.NextDouble();
            return _agent.Clock.Time >= TimeSpan.FromSeconds(randomTime);
        }

        private void SetStateAnalyze()
        {
            _agent.SetState(new TetrisAnalyzeState(_agent));
        }

        private void StartFromMenu(IExecutor actuator)
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
            SelectLevel(actuator, _startLevel);
        }

        private void StartFromGameOver(IExecutor actuator)
        {
            // sequence handles both cases (with and without entry in high score table)
            actuator.Hit(Button.Start);
            actuator.Hit(Button.Start);
            actuator.Hit(Button.B);
            actuator.Hit(Button.Start);
        }

        private void SelectLevel(IExecutor actuator, int startLevel)
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
