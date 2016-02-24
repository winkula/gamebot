using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core.Data
{
    public class DefaultCommand : ICommand
    {
        private readonly Buttons buttons;

        public DefaultCommand(Buttons buttons)
        {
            this.buttons = buttons;
        }

        public Buttons GetButtons()
        {
            return buttons;
        }

        public bool Press(Buttons button)
        {
            return buttons.HasFlag(button);
        }

        public bool PressUp()
        {
            return buttons.HasFlag(Buttons.Up);
        }

        public bool PressDown()
        {
            return buttons.HasFlag(Buttons.Down);
        }

        public bool PressLeft()
        {
            return buttons.HasFlag(Buttons.Left);
        }

        public bool PressRight()
        {
            return buttons.HasFlag(Buttons.Right);
        }

        public bool PressA()
        {
            return buttons.HasFlag(Buttons.A);
        }

        public bool PressB()
        {
            return buttons.HasFlag(Buttons.B);
        }
        
        public bool PressStart()
        {
            return buttons.HasFlag(Buttons.Start);
        }

        public bool PressSelect()
        {
            return buttons.HasFlag(Buttons.Select);
        }
    }
}
