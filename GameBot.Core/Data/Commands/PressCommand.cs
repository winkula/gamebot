using System;

namespace GameBot.Core.Data.Commands
{
    public class PressCommand : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public PressCommand(Button button, TimeSpan timestamp)
        {
            Button = button;
            Timestamp = timestamp;
        }

        public PressCommand(Button button) : this(button, TimeSpan.Zero)
        {
        }

        public void Execute(IActuator actuator)
        {
            actuator.Press(Button);
        }

        public override string ToString()
        {
            return $"PressCommand {{ Button: {Button}, Timestamp: {Timestamp} }}";
        }
    }
}
