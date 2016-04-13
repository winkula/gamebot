using System;

namespace GameBot.Core.Data.Commands
{
    public class HitCommand : ICommand
    {
        public Button Button { get; private set; }
        public TimeSpan? Press { get; private set; }
        public TimeSpan? Release { get; private set; }

        public HitCommand(Button button, TimeSpan timestamp, TimeSpan duration)
        {
            Button = button;
            Press = timestamp;
            Release = timestamp + duration;
        }

        public HitCommand(Button button, TimeSpan timestamp) : this(button, timestamp, TimeSpan.Zero)
        {
        }

        public HitCommand(Button button) : this(button, TimeSpan.Zero, TimeSpan.Zero)
        {
        }

        public override string ToString()
        {
            return $"HitCommand {{ Button: {Button}, Press: {Press}, Release: {Release} }}";
        }
    }
}
