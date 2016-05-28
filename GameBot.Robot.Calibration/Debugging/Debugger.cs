using GameBot.Core.Ui;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Robot.Ui.Debugging
{
    public class Debugger : IDebugger
    {
        private const int dynamicMax = 20;
        private readonly IList<string> messagesStatic = new List<string>();
        private Queue<string> messagesDynamic = new Queue<string>();
        
        public void WriteStatic(object message)
        {
            messagesStatic.Add(message.ToString());
        }

        public void WriteDynamic(object message)
        {
            messagesDynamic.Enqueue(message.ToString());
            while (messagesDynamic.Count > dynamicMax)
            {
                messagesDynamic.Dequeue();
            }
        }
        
        public IEnumerable<string> ReadStatic()
        {
            return messagesStatic.ToList();
        }

        public IEnumerable<string> ReadDynamic()
        {
            var list = messagesDynamic.ToList();
            list.Reverse();
            return list;
        }

        public void ClearStatic()
        {
            messagesStatic.Clear();
        }

        public void ClearDynamic()
        {
            messagesDynamic.Clear();
        }
    }
}
