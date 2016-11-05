﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameBot.Core.Data;

namespace GameBot.Core.Executors
{
    public class Executor : IExecutor
    {
        private readonly IActuator _actuator;
        private readonly IClock _clock;

        public Executor(IActuator actuator, IClock clock)
        {
            _actuator = actuator;
            _clock = clock;
        }
        
        public void Hit(Button button)
        {
            _actuator.Hit(button);
        }

        public void Hit(IEnumerable<Button> buttons)
        {
            _actuator.Hit(buttons);
        }

        public void Hit(Button button, TimeSpan duration)
        {
            _actuator.Press(button);
            _clock.Sleep((int) duration.TotalMilliseconds);
            _actuator.Release(button);
        }

        public async void HitAsync(Button button)
        {
            await Task.Run(() =>
            {
                _actuator.Hit(button);
            });
        }

        public async void HitAsync(Button button, TimeSpan duration)
        {
            await Task.Run(() =>
            {
                _actuator.Press(button);
                _clock.Sleep((int)duration.TotalMilliseconds);
                _actuator.Release(button);
            });
        }

        public void Press(Button button)
        {
            _actuator.Press(button);
        }

        public void Release(Button button)
        {
            _actuator.Release(button);
        }
    }
}
