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

        private readonly TimeSpan _buttonWaitDuration = TimeSpan.FromMilliseconds(100);

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
            var random = new Random();
            var randomTime = 2.5 + random.NextDouble();
            return _agent.Screenshot.Timestamp >= TimeSpan.FromSeconds(randomTime);
        }

        private void SetStateAnalyze()
        {
            _agent.SetState(new TetrisAnalyzeState(_agent));
        }

        private void StartFromMenu(IExecutor actuator)
        {
            // start 1 player mode
            actuator.HitWait(Button.Start, _buttonWaitDuration);

            // choose a-type
            actuator.HitWait(Button.A, _buttonWaitDuration);

            // choose music
            actuator.HitWait(Button.Right, _buttonWaitDuration);
            actuator.HitWait(Button.Down, _buttonWaitDuration);
            actuator.HitWait(Button.A, _buttonWaitDuration);

            // select level
            SelectLevel(actuator, _startLevel);
        }

        private void SelectLevel(IExecutor executor, int startLevel)
        {
            if (startLevel < 0 || startLevel > 9) throw new ArgumentException("startLevel must be between 0 and 9 (inclusive)");

            if (startLevel >= 5)
            {
                executor.HitWait(Button.Down, _buttonWaitDuration);
            }
            for (int i = 0; i < startLevel % 5; i++)
            {
                executor.HitWait(Button.Right, _buttonWaitDuration);
            }
            executor.HitWait(Button.A, _buttonWaitDuration);
        }

        private void StartFromGameOver(IExecutor executor)
        {
            // sequence handles both cases (with and without entry in high score table)

            executor.HitWait(Button.Start, _buttonWaitDuration);
            executor.HitWait(Button.B, _buttonWaitDuration);
            executor.HitWait(Button.Start, _buttonWaitDuration);
            executor.Hit(Button.Start);
        }
    }
}
