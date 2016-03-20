using GameBot.Core.Data;

namespace GameBot.Core.Agents
{
    public abstract class AbstractAgent<T> : IAgent where T : class, IGameState
    {
        protected readonly IExtractor<T> Extractor;
        protected readonly IDecider<T> Decider;

        protected readonly IContext<T> Context;

        public AbstractAgent(IExtractor<T> extractor, IDecider<T> decider)
        {
            Extractor = extractor;
            Decider = decider;
            Context = new Context<T>();
        }

        public ICommands Act(IScreenshot screenshot)
        {
            var gameState = Extractor.Extract(screenshot, Context);
            //Context.Add(gameState);

            var commands = Decider.Decide(gameState, Context);
            return commands;
        }
    }
}
