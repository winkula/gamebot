using System.Collections.Generic;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;

namespace GameBot.Engine.Emulated.Actuators
{
    public class EmulatedActuator : IActuator
    {
        private readonly Emulator _emulator;
        
        public EmulatedActuator(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void Hit(Button button)
        {
            lock (_emulator)
            {
                _emulator.Hit(button);
            }
        }

        public void Hit(IEnumerable<Button> buttons)
        {
            lock (_emulator)
            {
                _emulator.Hit(buttons);
            }
        }

        public void Press(Button button)
        {
            lock (_emulator)
            {
                _emulator.Press(button);
            }
        }

        public void Release(Button button)
        {
            lock (_emulator)
            {
                _emulator.Release(button);
            }
        }        
    }
}
