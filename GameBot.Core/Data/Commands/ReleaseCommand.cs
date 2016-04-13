using System;

namespace GameBot.Core.Data.Commands
{
    public class ReleaseCommand : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan? Press { get; private set; }
        public TimeSpan? Release { get; private set; }

        public ReleaseCommand(Button button, TimeSpan release)
        {
            Button = button;
            Release = release;
        }

        public ReleaseCommand(Button button) : this(button, TimeSpan.Zero)
        {
        }

        public override string ToString()
        {
            return $"ReleaseCommand {{ Button: {Button}, Press: {Press}, Release: {Release} }}";
        }
    }
}
