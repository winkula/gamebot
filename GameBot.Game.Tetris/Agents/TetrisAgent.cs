using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using System.Drawing;
using GameBot.Core.Ui;
using GameBot.Core.Data;
using GameBot.Game.Tetris.States;
using System;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Agents
{
    public class TetrisAgent : IAgent
    {
        // current state (state pattern)
        private ITetrisState State;

        // global services
        public IConfig Config { get; private set; }
        public IDebugger Debugger { get; private set; }
        public ITimeProvider TimeProvider { get; private set; }
        public TetrisExtractor Extractor { get; private set; }
        public TetrisAi Ai { get; private set; }
        public ISearch Search { get; private set; }
        public IScreenshot Screenshot { get; private set; }
        public IActuator Actuator { get; private set; }

        // global data
        public GameState GameState { get; private set; }

        public TetrisAgent(IConfig config, TetrisExtractor extractor, TetrisAi ai, ISearch search, ITimeProvider timeProvider, IDebugger debugger)
        {
            Config = config;
            Debugger = debugger;
            TimeProvider = timeProvider;
            Extractor = extractor;
            Ai = ai;
            Search = search;

            int startLevel = config.Read("Game.Tetris.StartLevel", 0);
            SetState(new TetrisStartState(startLevel));
        }
        
        public void SetState(ITetrisState newState)
        {
            if (newState == null)
                throw new ArgumentNullException("newState");

            State = newState;
        }

        public void Act(IScreenshot screenshot, IActuator actuator)
        {
            Screenshot = screenshot;
            Actuator = actuator;
            
            State.Act(this);
        }

        public IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);
            if (Extractor != null)
            {
                foreach (var rectangle in Extractor.Rectangles)
                {
                    visualization.Draw(new Rectangle(8 * rectangle.X, 8 * rectangle.Y, 8, 8), new Bgr(0, 0, 255), 1);
                }
                return visualization;
            }
            return image;
        }
    }
}
