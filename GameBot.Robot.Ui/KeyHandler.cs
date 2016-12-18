using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameBot.Robot.Ui
{
    public class KeyHandler
    {
        private readonly IDictionary<Keys, Action> _keyDownActions = new Dictionary<Keys, Action>();
        private readonly IDictionary<Keys, Action> _keyUpActions = new Dictionary<Keys, Action>();
        
        public void OnKeyDown(Keys key, Action action)
        {
            if (_keyDownActions.ContainsKey(key))
            {
                _keyDownActions[key] = action;
            }
            else
            {
                _keyDownActions.Add(key, action);
            }
        }

        public void OnKeyUp(Keys key, Action action)
        {
            if (_keyUpActions.ContainsKey(key))
            {
                _keyUpActions[key] = action;
            }
            else
            {
                _keyUpActions.Add(key, action);
            }
        }

        public void KeyDown(Keys keyCode)
        {
            Action action;
            if (_keyDownActions.TryGetValue(keyCode, out action))
            {
                action();
            }
        }

        public void KeyUp(Keys keyCode)
        {
            Action action;
            if (_keyUpActions.TryGetValue(keyCode, out action))
            {
                action();
            }
        }
    }
}
