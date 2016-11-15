using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using System.Drawing;
using GameBot.Core.Data;
using System;
using Emgu.CV.CvEnum;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Agents.States;
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
        public IConfig Config { get; private set; }
        public IQuantizer Quantizer { get; private set; }
        public IExecutor Executor { get; private set; }
        public IScreenshot Screenshot { get; private set; }
        public IExtractor Extractor { get; private set; }
        public ISearch Search { get; private set; }

        // data used by states
        public GameState GameState { get; set; }

        // config used by states
        public int CheckSamples { get; }
        public int ExtractionSamples { get; }

        // for visualization only
        public Piece ExtractedPiece { private get; set; }
        public Piece TracedPiece { private get; set; }
        public Tetrimino? ExtractedNextPiece { private get; set; }

        public TetrisAgent(IConfig config, IQuantizer quantizer, IExtractor extractor, ISearch search)
        {
            _visualize = config.Read("Game.Tetris.Visualize", false);

            Config = config;
            Quantizer = quantizer;
            Extractor = extractor;
            Search = search;

            CheckSamples = Config.Read("Game.Tetris.Check.Samples", 1);
            ExtractionSamples = Config.Read("Game.Tetris.Extractor.Samples", 1);
            //ExtractionUpperThreshold = Config.Read<double>("Game.Tetris.Extractor.UpperThreshold");
            //ExtractionLowerThreshold = Config.Read<double>("Game.Tetris.Extractor.LowerThreshold");

            Init();
        }

        private void Init()
        {
            var startLevel = Config.Read("Game.Tetris.StartLevel", 0);
            var startFromGameOver = Config.Read("Game.Tetris.StartFromGameOver", false);

            SetState(new TetrisStartState(this, startLevel, startFromGameOver));
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

            _state.Extract();
        }

        public IImage Visualize(IImage image)
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
                            Draw(visualization, tileCoordinates, Color.DodgerBlue);
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
                    Draw(visualization, tileCoordinates, Color.Orange);
                }
            }
            if (ExtractedPiece != null)
            {
                // current piece
                var piece = ExtractedPiece;
                foreach (var block in piece.Shape.Body)
                {
                    var tileCoordinates = Coordinates.PieceToTile(piece.X + block.X, piece.Y + block.Y);
                    Draw(visualization, tileCoordinates, Color.Red);
                }
            }
            if (ExtractedNextPiece != null)
            {
                // next piece
                foreach (var block in Shape.Get(ExtractedNextPiece.Value).Body)
                {
                    var tileCoordinates = Coordinates.PieceToTilePreview(block);
                    Draw(visualization, tileCoordinates, Color.LimeGreen);
                }
            }

            return visualization;
        }

        private void Draw(IImage image, Point tileCoordinates, Color color)
        {
            const int frameSize = 0;
            var rectangle = new Rectangle(GameBoyConstants.TileSize * tileCoordinates.X + frameSize, GameBoyConstants.TileSize * tileCoordinates.Y + frameSize, GameBoyConstants.TileSize - 2 * frameSize - 1, GameBoyConstants.TileSize - 2 * frameSize - 1);
            CvInvoke.Rectangle(image, rectangle, new Bgr(color).MCvScalar, 1, LineType.FourConnected);
        }

        public void Play(IExecutor executor)
        {
            Executor = executor;

            do
            {
                // every state can directly execute the next state, when it changes
                _continue = false;
                _state.Play();
            } while (_continue);
        }

        public void Reset()
        {
            Init();
        }
    }
}
