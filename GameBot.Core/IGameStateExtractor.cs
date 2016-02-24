using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core.Extractors
{
    public interface IGameStateExtractor
    {
    }

    public interface IGameStateExtractor<T> : IGameStateExtractor where T : IGameState, new()
    {
        T Extract(IDisplayState input);
    }
}
