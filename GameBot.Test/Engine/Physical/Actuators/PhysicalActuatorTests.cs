using System;
using System.Text;
using GameBot.Core.Data;
using GameBot.Engine.Physical.Actuators;
using NUnit.Framework;
using System.Threading;
using GameBot.Core;
using GameBot.Core.Configuration;
using NLog;

namespace GameBot.Test.Engine.Physical.Actuators
{
    [TestFixture]
    public class PhysicalActuatorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IConfig _config;

        [TestFixtureSetUp]
        public void Init()
        {
            _config = new AppSettingsConfig();
        }

        [Test]
        public void ConstructDispose()
        {
            using (var actuator = new PhysicalActuator(_config))
            {
            }
        }

        [Test]
        public void AllButtonsInSequence()
        {
            using (var actuator = new PhysicalActuator(_config))
            {
                actuator.Hit(Button.Up);
                Thread.Sleep(1000);

                actuator.Hit(Button.Down);
                Thread.Sleep(1000);

                actuator.Hit(Button.Left);
                Thread.Sleep(1000);

                actuator.Hit(Button.Right);
                Thread.Sleep(1000);

                actuator.Hit(Button.A);
                Thread.Sleep(1000);

                actuator.Hit(Button.B);
                Thread.Sleep(1000);

                actuator.Hit(Button.Start);
                Thread.Sleep(1000);

                actuator.Hit(Button.Select);
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void MenuTestRoutine()
        {
            using (var actuator = new PhysicalActuator(_config))
            {
                for (int i = 0; i < 10; i++)
                {
                    actuator.Hit(Button.Right);
                    actuator.Hit(Button.Left);

                    actuator.Hit(Button.A);

                    actuator.Hit(Button.Right);
                    actuator.Hit(Button.Down);
                    actuator.Hit(Button.Left);
                    actuator.Hit(Button.Up);

                    actuator.Hit(Button.B);
                }
            }
        }

        [Test]
        public void HighscoreRoutine()
        {
            var simulator = new HighscoreSimulator();
            var sequence = new []
            {
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up,
               
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up,

                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up,

                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up,

                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up,

                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up,

                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up, Button.A,
                Button.Up,

                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up, Button.B,
                Button.Up
            };

            using (var actuator = new PhysicalActuator(_config))
            {
                foreach (var button in sequence)
                {
                    actuator.Hit(button);
                    simulator.Handle(button);
                }
            }

            _logger.Info($"Expected result: {simulator.Result}");
        }
    }

    public class HighscoreSimulator
    {
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ.-+_";

        private const int _alphbethCount = 30;
        private readonly int[] _states = new int[6];
        private int _postition;
        private bool _ended;

        public void Handle(Button button)
        {
            if (!_ended)
            {
                switch (button)
                {
                    case Button.Up:
                        _states[_postition] = (_states[_postition] + 1) % _alphbethCount;
                        break;
                    case Button.Down:
                        _states[_postition] = (_states[_postition] - 1 + _alphbethCount) % _alphbethCount;
                        break;
                    case Button.A:
                        if (_postition == _states.Length - 1)
                        {
                            _ended = true;
                        }
                        else
                        {
                            _postition++;
                        }
                        break;
                    case Button.B:
                        _postition = Math.Max(0, _postition - 1);
                        break;
                }
            }
        }

        public string Result
        {
            get
            {
                var sb = new StringBuilder();

                foreach (int t in _states)
                {
                    sb.Append(_chars[t]);
                }

                return sb.ToString();
            }
        }
    }
}
