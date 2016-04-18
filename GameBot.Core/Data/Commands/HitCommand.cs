using System;

namespace GameBot.Core.Data.Commands
{
    public class HitCommand : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public HitCommand(Button button, TimeSpan timestamp)
        {
            Button = button;
            Timestamp = timestamp;
        }

        public HitCommand(Button button) : this(button, TimeSpan.Zero)
        {
        }

        public void Execute(IActuator actuator)
        {
            actuator.Hit(Button);
        }

        public override string ToString()
        {
            return $"HitCommand {{ Button: {Button}, Timestamp: {Timestamp} }}";
        }
    }
}
