using System;

namespace GameBot.Core.Data.Commands
{
    public class PressCommand : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan? Press { get; private set; }
        public TimeSpan? Release { get; private set; }

        public PressCommand(Button button, TimeSpan press)
        {
            Button = button;
            Press = press;
        }

        public PressCommand(Button button) : this(button, TimeSpan.Zero)
        {
        }

        public override string ToString()
        {
            return $"PressCommand {{ Button: {Button}, Press: {Press}, Release: {Release} }}";
        }
    }
}
