using System;

namespace GameBot.Core.Data
{
    public class Command : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan Duration { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public Command(Button button, TimeSpan duration, TimeSpan timestamp)
        {
            Button = button;
            Duration = duration;
            Timestamp = timestamp;
        }
    }
}
