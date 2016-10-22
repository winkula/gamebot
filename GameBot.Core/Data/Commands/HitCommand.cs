using System;

namespace GameBot.Core.Data.Commands
{
    public class HitCommand : ICommand
    {
        public Button Button { get; }
        public TimeSpan Timestamp { get; }

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
            return $"HitCommand {{ Button: {Button} }}";
        }
    }
}
