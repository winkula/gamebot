using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using System.Collections.Generic;

namespace GameBot.Core.Agents
{
    public abstract class AbstractAgent<T> : IAgent where T : class, IGameState
    {
        protected readonly IExtractor<T> Extractor;
        protected readonly ISolver<T> Solver;

        public AbstractAgent(IExtractor<T> extractor, ISolver<T> solver)
        {
            Extractor = extractor;
            Solver = solver;
        }

        public IEnumerable<ICommand> Act(IScreenshot screenshot)
        {
            if (MustExtract(screenshot))
            {
                var gameState = Extractor.Extract(screenshot);
                if (MustSolve(gameState))
                {
                    return Solver.Solve(gameState);
                }
            }
            return new CommandCollection();
        }

        protected virtual bool MustExtract(IScreenshot screenshot)
        {
            return true;
        }

        protected virtual bool MustSolve(T gameState)
        {
            return true;
        }
    }
}
