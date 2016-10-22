using GameBot.Core;
using System;
using GameBot.Core.Data;
using Tinkerforge;
using System.Threading;
using NLog;

namespace GameBot.Engine.Physical.Actuators
{
    public class PhysicalActuator : IActuator, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const int DelayHit = 60; // was 40
        private const int DelayCommand = 300; // was 50

        private readonly IConfig _config;

        private readonly string _host;
        private readonly int _port;
        private readonly string _uidMaster;
        private readonly string _uidRelay1;
        private readonly string _uidRelay2;
        private readonly string _uidTemp;

        private int _state1;
        private int _state2;

        private readonly IPConnection _ipcon;
        private readonly BrickMaster _master;
        private readonly BrickletIndustrialQuadRelay _or1;
        private readonly BrickletIndustrialQuadRelay _or2;
        private BrickletTemperatureIR _tir;

        public PhysicalActuator(IConfig config)
        {
            _config = config;

            _host = config.Read<string>("Robot.Actuator.Host");
            _port = config.Read<int>("Robot.Actuator.Port");
            _uidMaster = config.Read<string>("Robot.Actuator.UidMaster");
            _uidRelay1 = config.Read<string>("Robot.Actuator.UidRelay1");
            _uidRelay2 = config.Read<string>("Robot.Actuator.UidRelay2");
            _uidTemp = config.Read<string>("Robot.Actuator.UidTemp");
            
            _ipcon = new IPConnection(); // Create IP connection
            _ipcon.Connect(_host, _port); // Connect to brickd. Don't use device before ipcon is connected
            
            _master = new BrickMaster(_uidMaster, _ipcon); // Create device object
            _or1 = new BrickletIndustrialQuadRelay(_uidRelay1, _ipcon);
            _or2 = new BrickletIndustrialQuadRelay(_uidRelay2, _ipcon);
            _tir = new BrickletTemperatureIR(_uidTemp, _ipcon);
            
            // Get current stack voltage (unit is mV)
            int stackVoltage = _master.GetStackVoltage();
            _logger.Info("Stack Voltage: " + stackVoltage / 1000.0 + " V");

            // Get current stack current (unit is mA)
            int stackCurrent = _master.GetStackCurrent();
            _logger.Info("Stack Current: " + stackCurrent / 1000.0 + " A");

            //short ChipTemp = tir.GetAmbientTemperature();
            //logger.Info("Chibi master address: " + ChipTemp / 10 + "°/C");
        }

        public void Hit(Button button)
        {
            _logger.Info($"Hit button {button}");

            HandleState(button, true);
            Thread.Sleep(_delayHit);
            HandleState(button, false);
            Thread.Sleep(_delayCommand);
        }

        public void Press(Button button)
        {
            _logger.Info($"Press button {button}");

            HandleState(button, true);
            Thread.Sleep(_delayCommand);
        }

        public void Release(Button button)
        {
            _logger.Info($"Release button {button}");

            HandleState(button, false);
            Thread.Sleep(_delayCommand);
        }

        private void HandleState(Button button, bool pressOrRelease)
        {
            switch (button)
            {
                case Button.Up:
                    HandleState1Bit(1 << 2, pressOrRelease);
                    break;
                case Button.Down:
                    HandleState1Bit(1 << 1, pressOrRelease);
                    break;
                case Button.Left:
                    HandleState1Bit(1 << 0, pressOrRelease);
                    break;
                case Button.Right:
                    HandleState1Bit(1 << 3, pressOrRelease);
                    break;

                case Button.Start:
                    HandleState2Bit(1 << 3, pressOrRelease);
                    break;
                case Button.A:
                    HandleState2Bit(1 << 2, pressOrRelease);
                    break;
                case Button.Select:
                    HandleState2Bit(1 << 1, pressOrRelease);
                    break;
                case Button.B:
                    HandleState2Bit(1 << 0, pressOrRelease);
                    break;

                default:
                    throw new ArgumentException($"Undefined button {button}!");
            }
        }

        private void HandleState1Bit(int bitmask, bool pressOrRelease)
        {
            if (pressOrRelease)
            {
                _state1 |= bitmask;
            }
            else
            {
                _state1 &= ~bitmask;
            }
            _or1.SetValue(_state1);
        }

        private void HandleState2Bit(int bitmask, bool pressOrRelease)
        {
            if (pressOrRelease)
            {
                _state2 |= bitmask;
            }
            else
            {
                _state2 &= ~bitmask;
            }
            _or2.SetValue(_state2);
        }

        public void Dispose()
        {
            _or1.SetValue(0);
            _or2.SetValue(0);
            _ipcon.Disconnect();
        }
    }
}
