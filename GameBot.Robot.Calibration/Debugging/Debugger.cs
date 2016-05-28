using GameBot.Core.Ui;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Robot.Ui.Debugging
{
    public class Debugger : IDebugger
    {
        private readonly Queue<string> messages = new Queue<string>();

        public void Clear()
        {
            messages.Clear();
        }

        public IEnumerable<string> Read()
        {
            return messages.ToList();
        }

        public void Write(object message)
        {
            messages.Enqueue(message.ToString());
        }
    }
}
