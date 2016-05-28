using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using System.Collections.Generic;
using Emgu.CV;

namespace GameBot.Core.Agents
{
    public abstract class AbstractAgent<T> : IAgent where T : class, IGameState
    {
        protected readonly IExtractor<T> Extractor;
        protected readonly IPlayer<T> Player;

        public AbstractAgent(IExtractor<T> extractor, IPlayer<T> player)
        {
            Extractor = extractor;
            Player = player;
        }

        public IEnumerable<ICommand> Initialize()
        {
            return Player.Initialize();
        }

        public IEnumerable<ICommand> Act(IScreenshot screenshot)
        {
            if (MustExtract(screenshot))
            {
                var gameState = Extractor.Extract(screenshot);
                if (MustPlay(gameState))
                {
                    return Player.Play(gameState);
                }
            }
            return new CommandCollection();
        }
        
        public virtual IImage Visualize(IImage image)
        {
            return image;
        }

        protected virtual bool MustExtract(IScreenshot screenshot)
        {
            return true;
        }

        protected virtual bool MustPlay(T gameState)
        {
            return true;
        }
    }
}
