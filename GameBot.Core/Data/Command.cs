using System;

namespace GameBot.Core.Data
{
    public class Command : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }

        public Command(Button button, TimeSpan timestamp, TimeSpan duration)
        {
            Button = button;
            Timestamp = timestamp;
            Duration = duration;
        }

        public Command(Button button, TimeSpan timestamp) : this(button, timestamp, TimeSpan.Zero)
        {
        }

        public Command(Button button) : this(button, TimeSpan.Zero, TimeSpan.Zero)
        {
        }
    }
}
