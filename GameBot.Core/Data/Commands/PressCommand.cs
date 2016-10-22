using System;

namespace GameBot.Core.Data.Commands
{
    public class PressCommand : ICommand
    {
        public Button Button { get; }
        public TimeSpan Timestamp { get; }

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
            return $"PressCommand {{ Button: {Button} }}";
        }
    }
}
