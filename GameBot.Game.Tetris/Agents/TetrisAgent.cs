using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using System.Drawing;
using GameBot.Core.Data;
using System;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Agents.States;

namespace GameBot.Game.Tetris.Agents
{
    public class TetrisAgent : IAgent
    {
        // current state of the ai (state pattern)
        private ITetrisState _state;
        
        // global services
        public IConfig Config { get; private set; }

        public IClock Clock { get; private set; }
        public IExecutor Executor { get; private set; }
        public IScreenshot Screenshot { get; private set; }

        public TetrisExtractor Extractor { get; private set; }
        public PieceExtractor PieceExtractor { get; private set; }
        public ISearch Search { get; private set; }

        // global data
        public GameState GameState { get; set; }

        public TetrisAgent(IConfig config, TetrisExtractor extractor, PieceExtractor pieceExtractor, ISearch search, IClock clock)
        {
            Config = config;
            Clock = clock;
            Extractor = extractor;
            PieceExtractor = pieceExtractor;
            Search = search;

            Init();
        }

        private void Init()
        {
            int startLevel = Config.Read("Game.Tetris.StartLevel", 0);
            bool startFromGameOver = Config.Read("Game.Tetris.StartFromGameOver", false);
            SetState(new TetrisStartState(this, startLevel, startFromGameOver));
        }
        
        public void SetState(ITetrisState newState)
        {
            if (newState == null)
                throw new ArgumentNullException("newState");

            _state = newState;
        }

        public void Act(IScreenshot screenshot, IExecutor executor)
        {
            Screenshot = screenshot;
            Executor = executor;
            
            if (Extractor != null)
            {
                Extractor.Rectangles.Clear();
            }

            _state.Act();
        }

        public IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);
            if (Extractor != null)
            {
                foreach (var rectangle in Extractor.Rectangles)
                {
                    visualization.Draw(new Rectangle(8 * rectangle.X, 8 * rectangle.Y, 8, 8), new Bgr(0, 0, 255), 2);
                }
                return visualization;
            }
            return image;
        }

        public void Reset()
        {
            Init();
        }
    }
}
