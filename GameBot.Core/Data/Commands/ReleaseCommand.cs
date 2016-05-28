using System;

namespace GameBot.Core.Data.Commands
{
    public class ReleaseCommand : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public ReleaseCommand(Button button, TimeSpan timestamp)
        {
            Button = button;
            Timestamp = timestamp;
        }

        public ReleaseCommand(Button button) : this(button, TimeSpan.Zero)
        {
        }

        public void Execute(IActuator actuator)
        {
            actuator.Release(Button);
        }

        public override string ToString()
        {
            return $"ReleaseCommand {{ Button: {Button} }}";
        }
    }
}
