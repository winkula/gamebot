using System;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.States
{
    public class StartState : BaseState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan _buttonWaitDuration = TimeSpan.FromMilliseconds(100);
        
        private readonly bool _heartMode;
        private readonly bool _startFromGameover;
        private readonly int _startLevel;
        
        public StartState(TetrisAgent agent, int startLevel, bool heartMode, bool startFromGameOver) : base(agent)
        {
            _heartMode = heartMode;
            _startFromGameover = startFromGameOver;
            _startLevel = startLevel;
        }

        // constructor, when we start again from game over
        public StartState(TetrisAgent agent, GameState gameState) : this(agent, gameState.StartLevel, gameState.HeartMode, true)
        {
        }

        public override void Extract()
        {
            // do nothing
        }

        public override void Play()
        {
            if (IsStartScreenVisble())
            {
                if (_startFromGameover)
                {
                    _logger.Info("Game started from game over screen");

                    // handle start menu
                    StartFromGameOver(Agent.Executor);
                }
                else
                {
                    _logger.Info("Game started from start menu");

                    // restart from game over screen (good for testing multiple games)
                    StartFromMenu(Agent.Executor);
                }

                SetStateAnalyze();
            }
        }

        private bool IsStartScreenVisble()
        {
            var random = new Random();
            var randomTime = 2.5 + random.NextDouble();
            return Screenshot.Timestamp >= TimeSpan.FromSeconds(randomTime);
        }

        private void SetStateAnalyze()
        {
            SetState(new AnalyzeState(Agent, Agent.Clock.Time));
        }

        private void StartFromMenu(IExecutor executor)
        {
            if (_heartMode) executor.Press(Button.Down);

            // start 1 player mode
            executor.HitWait(Button.Start, _buttonWaitDuration);

            if (_heartMode) executor.Release(Button.Down);

            // choose a-type
            executor.HitWait(Button.A, _buttonWaitDuration);

            // choose music
            executor.HitWait(Button.Right, _buttonWaitDuration);
            executor.HitWait(Button.Down, _buttonWaitDuration);
            executor.HitWait(Button.A, _buttonWaitDuration);

            // select level
            SelectLevel(executor, _startLevel);
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
            executor.Hit(Button.A);
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
