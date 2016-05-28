using System.Collections.Generic;

namespace GameBot.Core.Ui
{
    public interface IDebugger
    {
        void Write(object message);

        IEnumerable<string> Read();

        void Clear();
    }
}
