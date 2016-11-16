using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Data;
using Tinkerforge;
using System.Threading;
using NLog;

namespace GameBot.Engine.Physical.Actuators
{
    public class PhysicalActuator : IActuator, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int _delayHold = 35;
        private const int _delayReleased = 40;
        
        private readonly IPConnection _ipcon;
        private readonly BrickletIndustrialQuadRelay _or1;
        private readonly BrickletIndustrialQuadRelay _or2;

        private int _state1;
        private int _state2;

        private bool _isDirtyOr1;
        private bool _isDirtyOr2;

        public PhysicalActuator(IConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var host = config.Read<string>("Robot.Actuator.Host");
            var port = config.Read<int>("Robot.Actuator.Port");
            var uidMaster = config.Read<string>("Robot.Actuator.UidMaster");
            var uidRelay1 = config.Read<string>("Robot.Actuator.UidRelay1");
            var uidRelay2 = config.Read<string>("Robot.Actuator.UidRelay2");

            _ipcon = new IPConnection(); // Create IP connection
            _ipcon.Connect(host, port); // Connect to brickd. Don't use device before ipcon is connected

            var master = new BrickMaster(uidMaster, _ipcon);
            _or1 = new BrickletIndustrialQuadRelay(uidRelay1, _ipcon);
            _or2 = new BrickletIndustrialQuadRelay(uidRelay2, _ipcon);

            // Get current stack voltage (unit is mV)
            int stackVoltage = master.GetStackVoltage();
            _logger.Info($"Stack Voltage: {stackVoltage / 1000.0:F} V");

            // Get current stack current (unit is mA)
            int stackCurrent = master.GetStackCurrent();
            _logger.Info($"Stack Current: {stackCurrent / 1000.0:F} A");
        }

        public void Hit(Button button)
        {
            _logger.Info($"Hit button {button}");

            HandleState(button, true);
            Update();
            Thread.Sleep(_delayHold);
            HandleState(button, false);
            Update();
            Thread.Sleep(_delayReleased);
        }

        public void Hit(IEnumerable<Button> buttons)
        {
            var buttonsList = buttons.ToList();
            _logger.Info($"Hit buttons {string.Join(", ", buttonsList)}");

            foreach (var button in buttonsList)
            {
                HandleState(button, true);
            }
            Update();
            Thread.Sleep(_delayHold);
            foreach (var button in buttonsList)
            {
                HandleState(button, false);
            }
            Update();
            Thread.Sleep(_delayReleased);
        }

        public void Press(Button button)
        {
            _logger.Info($"Press button {button}");

            HandleState(button, true);
            Update();
        }

        public void Release(Button button)
        {
            _logger.Info($"Release button {button}");

            HandleState(button, false);
            Update();
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
            _isDirtyOr1 = true;
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
            _isDirtyOr2 = true;
        }

        private void Update()
        {
            if (_isDirtyOr1)
            {
                _or1.SetValue(_state1);
                _isDirtyOr1 = false;
            }
            if (_isDirtyOr2)
            {
                _or2.SetValue(_state2);
                _isDirtyOr2 = false;
            }
        }

        public void Dispose()
        {
            _or1.SetValue(0);
            _or2.SetValue(0);
            _ipcon.Disconnect();
        }
    }
}
