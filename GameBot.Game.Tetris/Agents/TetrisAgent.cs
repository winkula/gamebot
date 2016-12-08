using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using System.Drawing;
using GameBot.Core.Data;
using System;
using Emgu.CV.CvEnum;
using GameBot.Core.Exceptions;
using GameBot.Core.Extensions;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Agents.States;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Extractors;

namespace GameBot.Game.Tetris.Agents
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TetrisAgent : IAgent
    {
        private readonly bool _visualize;

        // state informations
        private ITetrisAgentState _state;
        private bool _continue;

        // services and data used by states
        public IConfig Config { get; }
        public IClock Clock { get; private set; }
        public IQuantizer Quantizer { get; private set; }
        public IExecutor Executor { get; private set; }
        public IScreenshot Screenshot { get; private set; }
        public IExtractor Extractor { get; private set; }
        public IBoardExtractor BoardExtractor { get; private set; }
        public ISearch Search { get; private set; }

        // data used by states
        public GameState GameState { get; set; }

        // config used by states
        public int ExtractionSamples { get; }
        public bool PlayMultiplayer { get; }
        public bool CheckEnabled { get; }
        
        #region timing

        // timing config
        private readonly TimeSpan _hitTime;
        private readonly TimeSpan _hitDelayAfter;

        public readonly TimeSpan MoreTimeToAnalyze;
        public readonly TimeSpan LessFallTimeBeforeDrop;
        public readonly TimeSpan LessWaitTimeAfterDrop;

        public TimeSpan GetExecutionDuration(int commands)
        {
            // the drop command is not counted here, because
            // it has no delay time (press and release, no hit)
            commands = Math.Max(0, commands - 1);

            return (_hitTime + _hitDelayAfter).Multiply(commands);
        }

        #endregion

        // for visualization only
        public Piece ExtractedPiece { private get; set; }
        public Piece TracedPiece { private get; set; }
        public Tetrimino? ExtractedNextPiece { private get; set; }
        public int SearchHeight { private get; set; }

        public TetrisAgent(IConfig config, IClock clock, IQuantizer quantizer, IExtractor extractor, IBoardExtractor boardExtractor, ISearch search)
        {
            _visualize = config.Read("Game.Tetris.Visualize", false);

            Config = config;
            Clock = clock;
            Quantizer = quantizer;
            Extractor = extractor;
            BoardExtractor = boardExtractor;
            Search = search;
            
            ExtractionSamples = config.Read("Game.Tetris.Extractor.Samples", 1);
            PlayMultiplayer = config.Read("Game.Tetris.Multiplayer", false);
            CheckEnabled = config.Read("Game.Tetris.Check.Enabled", false);

            // init timing config
            _hitTime = TimeSpan.FromMilliseconds(Config.Read<int>("Robot.Actuator.Hit.Time"));
            _hitDelayAfter = TimeSpan.FromMilliseconds(Config.Read<int>("Robot.Actuator.Hit.DelayAfter"));
            MoreTimeToAnalyze = TimeSpan.FromMilliseconds(Config.Read<int>("Game.Tetris.Timing.MoreTimeToAnalyze"));
            LessFallTimeBeforeDrop = TimeSpan.FromMilliseconds(Config.Read<int>("Game.Tetris.Timing.LessFallTimeBeforeDrop"));
            LessWaitTimeAfterDrop = TimeSpan.FromMilliseconds(Config.Read<int>("Game.Tetris.Timing.LessWaitTimeAfterDrop"));
                
            Init();
        }

        private void Init()
        {
            ResetVisualization();

            var startLevel = Config.Read("Game.Tetris.StartLevel", 0);
            var heartMode = Config.Read("Game.Tetris.HeartMode", false);
            var startFromGameOver = Config.Read("Game.Tetris.StartFromGameOver", false);

            SetState(new TetrisStartState(this, startLevel, heartMode, startFromGameOver));
        }

        private void ResetVisualization()
        {
            ExtractedPiece = null;
            ExtractedNextPiece = null;
            TracedPiece = null;
            SearchHeight = 0;
        }

        public void SetState(ITetrisAgentState newState)
        {
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            _state = newState;
            _continue = false;
        }

        public void SetStateAndContinue(ITetrisAgentState newState)
        {
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            _state = newState;
            _continue = true;
        }

        public void Extract(IScreenshot screenshot)
        {
            Screenshot = screenshot;

            try
            {
                _state.Extract();
            }
            catch (GameOverException)
            {
                // game over detected
                SetStateAndContinue(new TetrisStartState(this, GameState));
            }
        }

        public Mat Visualize(Mat image)
        {
            if (!_visualize)
            {
                // no visualization
                return image;
            }

            var visualization = new Mat();
            CvInvoke.CvtColor(image, visualization, ColorConversion.Gray2Bgr);

            if (GameState?.Board != null)
            {
                var board = GameState.Board;
                for (int x = 0; x < board.Width; x++)
                {
                    for (int y = 0; y < board.Height; y++)
                    {
                        if (board.IsOccupied(x, y))
                        {
                            var tileCoordinates = Coordinates.BoardToTile(x, y);
                            DrawBlock(visualization, tileCoordinates, Color.DodgerBlue);
                        }
                    }
                }
            }
            if (TracedPiece != null)
            {
                // current piece
                var piece = TracedPiece;
                foreach (var block in piece.Shape.Body)
                {
                    var tileCoordinates = Coordinates.PieceToTile(piece.X + block.X, piece.Y + block.Y);
                    DrawBlock(visualization, tileCoordinates, Color.Orange);
                }
            }
            if (ExtractedPiece != null)
            {
                // current piece
                var piece = ExtractedPiece;
                foreach (var block in piece.Shape.Body)
                {
                    var tileCoordinates = Coordinates.PieceToTile(piece.X + block.X, piece.Y + block.Y);
                    DrawBlock(visualization, tileCoordinates, Color.Red);
                }
            }
            if (ExtractedNextPiece != null)
            {
                // next piece
                foreach (var block in Shape.Get(ExtractedNextPiece.Value).Body)
                {
                    var tileCoordinates = Coordinates.PieceToTilePreview(block);
                    DrawBlock(visualization, tileCoordinates, Color.LimeGreen);
                }
            }
            if (SearchHeight > 0)
            {
                DrawLine(visualization, SearchHeight, Color.Red);
            }

            return visualization;
        }

        private void DrawBlock(Mat image, Point tileCoordinates, Color color)
        {
            const int frameSize = 0;
            var rectangle = new Rectangle(GameBoyConstants.TileSize * tileCoordinates.X + frameSize, GameBoyConstants.TileSize * tileCoordinates.Y + frameSize, GameBoyConstants.TileSize - 2 * frameSize - 1, GameBoyConstants.TileSize - 2 * frameSize - 1);
            CvInvoke.Rectangle(image, rectangle, new Bgr(color).MCvScalar, 1, LineType.FourConnected);
        }

        private void DrawLine(Mat image, int height, Color color)
        {
            var pointFrom = new Point(2 * GameBoyConstants.TileSize, GameBoyConstants.TileSize * (height + 3));
            var pointTo = new Point(12 * GameBoyConstants.TileSize - 1, GameBoyConstants.TileSize * (height + 3));
            CvInvoke.Line(image, pointFrom, pointTo, new Bgr(color).MCvScalar, 1, LineType.FourConnected);
        }

        public void Play(IExecutor executor)
        {
            Executor = executor;

            do
            {
                // every state can directly execute the next state,
                // when it changes this flag to true (with SetStateAndContinue)
                _continue = false;

                try
                {
                    _state.Play();
                }
                catch (GameOverException)
                {
                    // game over detected
                    SetStateAndContinue(new TetrisStartState(this, GameState));
                }

            } while (_continue);
        }

        public void Reset()
        {
            Init();
        }
    }
}
