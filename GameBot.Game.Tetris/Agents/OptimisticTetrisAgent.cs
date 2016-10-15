using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using System.Drawing;
using System;
using GameBot.Core.Data;
using System.Collections.Generic;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.Agents
{
    public class OptimisticTetrisAgent : IAgent
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IActuator _actuator;
        private readonly IClock _clock;
        private readonly TetrisExtractor _extractor;
        private readonly TetrisAi _ai;

        private bool _initialized = false;
        private bool _awaitNextTetromino = true;
        private TimeSpan _timeNextAction = TimeSpan.Zero;
        
        public OptimisticTetrisAgent(IActuator actuator, TetrisExtractor extractor, TetrisAi ai, IClock clock)
        {
            _actuator = actuator;
            _clock = clock;
            _extractor = extractor;
            _ai = ai;
        }

        public void Act(IScreenshot screenshot, IExecutor executor)
        {
            var gameState = _extractor.Extract(screenshot, _ai.CurrentGameState);
            
            if (!_initialized)
            {
                _initialized = true;
                var commands = _ai.Initialize();
                Execute(commands, executor);
            }
            else if (MustPlay(gameState))
            {
                var commands = _ai.Play(gameState);
                Execute(commands, executor);
                AfterPlay();
            }
        }

        private void Execute(IEnumerable<ICommand> commands, IExecutor executor)
        {
            if (commands != null)
            {
                foreach (var nextCommand in commands)
                {
                    nextCommand.Execute(_actuator);
                }
            }
        }

        private bool MustPlay(GameState gameState)
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));

            if (gameState.Piece == null && _timeNextAction <= _clock.Time)
            {
                // await next tetromino when the piece was null some time in the past
                // and the timer to look for the next piece is exceeded
                _awaitNextTetromino = true;
            }
            
            _logger.Info("Current game state: " + _ai.CurrentGameState);

            return _awaitNextTetromino &&
                gameState.Piece != null &&
                gameState.NextPiece != null;
        }

        public void AfterPlay()
        {
            // start next timer
            var duration = TetrisLevel.GetFreeFallDuration(_ai.LastWay.Fall);

            _timeNextAction = _clock.Time.Add(duration);
            _awaitNextTetromino = false;
        }

        public IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);
            var tetrisExtractor = _extractor as TetrisExtractor;
            if (tetrisExtractor != null)
            {
                foreach (var rectangle in tetrisExtractor.Rectangles)
                {
                    visualization.Draw(new Rectangle(8 * rectangle.X, 8 * rectangle.Y, 8, 8), new Bgr(0, 0, 255), 1);
                }
                return visualization;
            }
            return image;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
