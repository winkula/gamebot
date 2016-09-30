using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using System.Collections.Generic;
using Emgu.CV;
using System;

namespace GameBot.Core.Agents
{
    public abstract class AbstractAgent<T> : IAgent where T : class, IGameState
    {
        protected readonly IExtractor<T> Extractor;
        protected readonly IPlayer<T> Player;
        protected bool initialized = false;

        public AbstractAgent(IExtractor<T> extractor, IPlayer<T> player)
        {
            Extractor = extractor;
            Player = player;
        }

        public IEnumerable<ICommand> Act(IScreenshot screenshot)
        {
            var gameState = Extractor.Extract(screenshot);

            if (!initialized)
            {
                initialized = true;
                return Player.Initialize();
            }
            if (MustPlay(gameState))
            {
                var commands = Play(gameState);
                AfterPlay();
                return commands;
            }

            return new CommandCollection();
        }

        public bool Check(ICommand command)
        {
            throw new NotImplementedException();
        }

        public virtual bool MustPlay(T gameState)
        {
            return true;
        }

        public virtual void AfterPlay()
        {
        }

        public virtual IEnumerable<ICommand> Play(T gameState)
        {
            return Player.Play(gameState);
        }

        public virtual IImage Visualize(IImage image)
        {
            return image;
        }
    }
}
