using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core
{
    public interface ICommand
    {
        Buttons GetButtons();
        bool Press(Buttons button);
        bool PressUp();
        bool PressDown();
        bool PressLeft();
        bool PressRight();
        bool PressA();
        bool PressB();
        bool PressStart();
        bool PressSelect();
    }
}
