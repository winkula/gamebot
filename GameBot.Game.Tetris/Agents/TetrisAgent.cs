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
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TetrisAgent : IAgent
    {
        // current state of the ai (state pattern)
        private ITetrisState _state;

        // global services
        public IConfig Config { get; private set; }

        public IClock Clock { get; private set; }
        public IExecutor Executor { get; private set; }
        public IScreenshot Screenshot { get; private set; }

        public PieceExtractor PieceExtractor { get; private set; }
        public ISearch Search { get; private set; }

        public double ProbabilityThreshold { get; }

        // global data
        public GameState GameState { get; set; }

        // for visualization only
        public Piece ExtractedPiece { get; set; }
        public Tetromino? ExtractedNextPiece { get; set; }

        public TetrisAgent(IConfig config, PieceExtractor pieceExtractor, ISearch search, IClock clock)
        {
            Config = config;
            Clock = clock;
            PieceExtractor = pieceExtractor;
            Search = search;

            ProbabilityThreshold = Config.Read<double>("Game.Tetris.Extractor.ProbabilityThreshold");

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
                throw new ArgumentNullException(nameof(newState));

            _state = newState;
        }

        public void Act(IScreenshot screenshot, IExecutor executor)
        {
            Screenshot = screenshot;
            Executor = executor;

            _state.Act();
        }

        public IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);

            if (ExtractedPiece != null)
            {
                // current piece
                foreach (var block in ExtractedPiece.Shape.Body)
                {
                    var tileCoordinates = Coordinates.PieceToTile(ExtractedPiece.X + block.X, ExtractedPiece.Y + block.Y);
                    int x = tileCoordinates.X;
                    int y = tileCoordinates.Y;

                    visualization.Draw(new Rectangle(8 * x, 8 * y, 8, 8), new Bgr(0, 255, 0), 2);
                }
            }

            if (ExtractedNextPiece != null)
            {
                // next piece
                foreach (var block in Shape.Get(ExtractedNextPiece.Value).Body)
                {
                    var tileCoordinates = Coordinates.PieceToTilePreview(block);
                    int x = tileCoordinates.X;
                    int y = tileCoordinates.Y;

                    visualization.Draw(new Rectangle(8 * x, 8 * y, 8, 8), new Bgr(0, 0, 255), 2);
                }
            }

            return visualization;
        }

        public void Reset()
        {
            Init();
        }
    }
}
