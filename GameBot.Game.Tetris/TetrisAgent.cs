using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Exceptions;
using GameBot.Core.Extensions;
using GameBot.Game.Tetris.Commands;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Extractors;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.States;

namespace GameBot.Game.Tetris
{
    public class TetrisAgent : IAgent
    {
        private readonly bool _visualize;

        // state informations
        private IState _state;
        private bool _continue;

        // services and data used by states
        public IConfig Config { get; }
        public IClock Clock { get; private set; }
        public IQuantizer Quantizer { get; private set; }
        public IExecutor Executor { get; private set; }
        public IScreenshot Screenshot { get; private set; }
        public IExtractor Extractor { get; private set; }
        public IBoardExtractor BoardExtractor { get; private set; }
        public IScreenExtractor ScreenExtractor { get; private set; }
        public ISearch Search { get; private set; }

        // data used by states
        public GameState GameState { get; set; }

        #region config

        public bool IsVisualize => Config.Read("Game.Tetris.Visualize", false);
        public int StartLevel => Config.Read("Game.Tetris.StartLevel", 0);
        public int ExtractionSamples => Config.Read("Game.Tetris.Extractor.Samples", 1);
        public bool IsMultiplayer => Config.Read("Game.Tetris.Multiplayer", false);
        public bool CheckEnabled => Config.Read("Game.Tetris.Check.Enabled", false);
        public bool IsHeartMode => Config.Read("Game.Tetris.HeartMode", false);

        #endregion

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

        public TetrisAgent(IConfig config, IClock clock, IQuantizer quantizer, IExecutor exceutor, IExtractor extractor, IBoardExtractor boardExtractor, IScreenExtractor screenExtractor, ISearch search)
        {
            Config = config;
            Clock = clock;
            Quantizer = quantizer;
            Executor = exceutor;
            Extractor = extractor;
            BoardExtractor = boardExtractor;
            ScreenExtractor = screenExtractor;
            Search = search;

            _visualize = IsVisualize;

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
            
            // init game state and agent state
            if (IsMultiplayer)
            {
                // TODO: is it correct that the game in multiplayer mode starts always in level 0?
                GameState = new GameState { StartLevel = 0, HeartMode = false };
            }
            else
            {
                GameState = new GameState { StartLevel = StartLevel, HeartMode = IsHeartMode };
            }

            SetState(new ReadyState(this));
        }

        private void ResetVisualization()
        {
            ExtractedPiece = null;
            ExtractedNextPiece = null;
            TracedPiece = null;
            SearchHeight = 0;
        }

        public void SetState(IState newState)
        {
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            _state = newState;
            _continue = false;
        }

        public void SetStateAndContinue(IState newState)
        {
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            _state = newState;
            _continue = true;
        }

        public void Extract(IScreenshot screenshot)
        {
            Screenshot = screenshot;

            do
            {
                // every state can directly execute the next state,
                // when it changes this flag to true (with SetStateAndContinue)
                _continue = false;

                try
                {
                    _state.Extract();
                }
                catch (GameOverException)
                {
                    // game over detected
                    SetState(new GameOverState(this));
                    throw; // let the engine decide what to do
                }

            } while (_continue);
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
                    SetState(new GameOverState(this));
                    throw; // let the engine decide what to do
                }

            } while (_continue);
        }

        public void Send(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                switch (message)
                {
                    case "reset":
                        Init();
                        break;
                    case "select level":
                        new SelectLevelCommand(Executor, StartLevel).Execute();
                        break;
                    case "highscore":
                        new HighscoreCommand(Executor, "THEBOT").Execute();
                        break;
                    case "menu":
                        new HeartModeCommand(Executor, IsHeartMode).Execute();
                        break;
                    case "start from game over":
                        new StartFromGameOverCommand(Executor).Execute();
                        break;
                }
            }
        }
    }
}
