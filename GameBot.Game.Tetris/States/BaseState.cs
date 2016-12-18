using System;
using GameBot.Core.Data;
using GameBot.Core.Exceptions;

namespace GameBot.Game.Tetris.States
{
    public abstract class BaseState : IState
    {
        protected TetrisAgent Agent { get; }
        protected IScreenshot Screenshot => Agent.Screenshot;

        protected BaseState(TetrisAgent agent)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            Agent = agent;
        }

        public virtual void Extract()
        {
            // try to detect game over
            DetectGameOver();
        }

        public virtual void Play()
        {
        }

        private void DetectGameOver()
        {
            if (Agent.IsMultiplayer)
            {
                if (Agent.ScreenExtractor.IsGameOverMultiplayer(Screenshot))
                {
                    throw new GameOverException();
                }
            }
            else
            {
                if (Agent.ScreenExtractor.IsGameOverSingleplayer(Screenshot))
                {
                    throw new GameOverException();
                }
            }
        }

        protected void SetState(IState newState)
        {
            Agent.SetState(newState);
        }

        protected void SetStateAndContinue(IState newState)
        {
            Agent.SetStateAndContinue(newState);
        }
    }
}
