using GameBot.Core;
using System;
using GameBot.Core.Data;
using Tinkerforge;
using System.Diagnostics;
using System.Threading;

namespace GameBot.Robot.Actuators
{
    public class Actuator : IActuator, IDisposable
    {
        private readonly IConfig config;

        private string host;
        private int port;
        private string uidMaster;
        private string uidRelay1;
        private string uidRelay2;
        private string uidTemp;

        private int State1;
        private int State2;

        private IPConnection ipcon;
        private BrickMaster master;
        private BrickletIndustrialQuadRelay or1;
        private BrickletIndustrialQuadRelay or2;
        private BrickletTemperatureIR tir;

        public Actuator(IConfig config)
        {
            this.config = config;

            this.host = config.Read<string>("Robot.Actuator.Host");
            this.port = config.Read<int>("Robot.Actuator.Port");
            this.uidMaster = config.Read<string>("Robot.Actuator.UidMaster");
            this.uidRelay1 = config.Read<string>("Robot.Actuator.UidRelay1");
            this.uidRelay2 = config.Read<string>("Robot.Actuator.UidRelay2");
            this.uidTemp = config.Read<string>("Robot.Actuator.UidTemp");
            
            ipcon = new IPConnection(); // Create IP connection
            ipcon.Connect(host, port); // Connect to brickd. Don't use device before ipcon is connected
            
            master = new BrickMaster(uidMaster, ipcon); // Create device object
            or1 = new BrickletIndustrialQuadRelay(uidRelay1, ipcon);
            or2 = new BrickletIndustrialQuadRelay(uidRelay2, ipcon);
            tir = new BrickletTemperatureIR(uidTemp, ipcon);
            
            // Get current stack voltage (unit is mV)
            int stackVoltage = master.GetStackVoltage();
            Debug.WriteLine("Stack Voltage: " + stackVoltage / 1000.0 + " V");

            // Get current stack current (unit is mA)
            int stackCurrent = master.GetStackCurrent();
            Debug.WriteLine("Stack Current: " + stackCurrent / 1000.0 + " A");

            short ChipTemp = tir.GetAmbientTemperature();
            Debug.WriteLine("Chibi master address: " + ChipTemp / 10 + "°/C");
        }

        public void Hit(Button button)
        {
            HandleState(button, true);
            Thread.Sleep(50);
            HandleState(button, false);
            Thread.Sleep(50);
        }

        public void Press(Button button)
        {
            HandleState(button, true);
            Thread.Sleep(50);
        }

        public void Release(Button button)
        {
            HandleState(button, false);
            Thread.Sleep(50);
        }

        private void HandleState(Button button, bool pressOrRelease)
        {
            switch (button)
            {
                case Button.Up:
                    HandleState1Bit(1 << 0, pressOrRelease);
                    break;
                case Button.Down:
                    HandleState1Bit(1 << 1, pressOrRelease);
                    break;
                case Button.Left:
                    HandleState1Bit(1 << 2, pressOrRelease);
                    break;
                case Button.Right:
                    HandleState1Bit(1 << 3, pressOrRelease);
                    break;

                case Button.Start:
                    HandleState2Bit(1 << 2, pressOrRelease);
                    break;
                case Button.A:
                    HandleState2Bit(1 << 1, pressOrRelease);
                    break;
                case Button.Select:
                    HandleState2Bit(1 << 0, pressOrRelease);
                    break;
                case Button.B:
                    HandleState2Bit(1 << 3, pressOrRelease);
                    break;

                default:
                    throw new Exception("Undefind button!");
            }
        }

        private void HandleState1Bit(int bitmask, bool pressOrRelease)
        {
            if (pressOrRelease)
            {
                State1 |= bitmask;
            }
            else
            {
                State1 &= ~bitmask;
            }
            or1.SetValue(State1);
        }

        private void HandleState2Bit(int bitmask, bool pressOrRelease)
        {
            if (pressOrRelease)
            {
                State2 |= bitmask;
            }
            else
            {
                State2 &= ~bitmask;
            }
            or2.SetValue(State2);
        }

        public void Dispose()
        {
            or1.SetValue(0);
            or2.SetValue(0);
            ipcon.Disconnect();
        }
    }
}
