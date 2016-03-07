using System.Collections.Generic;

namespace GameBot.Core.Searching
{
    public interface INode
    {
        int Depth { get; }

        INode Parent { get; }

        double GetScore();

        IEnumerable<INode> GetSuccessors();
    }
}
