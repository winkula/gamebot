using System.Collections.Generic;

namespace GameBot.Core.Ui
{
    public interface IDebugger
    {
        void WriteStatic(object message);

        void WriteDynamic(object message);

        IEnumerable<string> ReadStatic();

        IEnumerable<string> ReadDynamic();
        
        void ClearStatic();

        void ClearDynamic();
    }
}
