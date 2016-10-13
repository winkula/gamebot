using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;

namespace GameBot.Engine.Emulated.Actuators
{
    public class EmulatedActuator : IActuator
    {
        private readonly Emulator emulator;
        
        public EmulatedActuator(Emulator emulator)
        {
            this.emulator = emulator;
        }

        public void Hit(Button button)
        {
            emulator.Hit(button);
        }

        public void Press(Button button)
        {
            emulator.Press(button);
        }

        public void Release(Button button)
        {
            emulator.Release(button);
        }        
    }
}
