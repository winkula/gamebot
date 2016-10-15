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
            _emulator.Hit(button);
        }

        public void Press(Button button)
        {
            _emulator.Press(button);
        }

        public void Release(Button button)
        {
            _emulator.Release(button);
        }        
    }
}
