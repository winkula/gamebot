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
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IClock clock;
        private readonly TetrisExtractor extractor;
        private readonly TetrisAi ai;

        private bool initialized = false;
        private bool awaitNextTetromino = true;
        private TimeSpan timeNextAction = TimeSpan.Zero;
        
        public OptimisticTetrisAgent(TetrisExtractor extractor, TetrisAi ai, IClock clock)
        {
            this.clock = clock;
            this.extractor = extractor;
            this.ai = ai;
        }

        public void Act(IScreenshot screenshot, IActuator actuator)
        {
            var gameState = extractor.Extract(screenshot, ai.CurrentGameState);
            
            if (!initialized)
            {
                initialized = true;
                var commands = ai.Initialize();
                Execute(commands, actuator);
            }
            else if (MustPlay(gameState))
            {
                var commands = ai.Play(gameState);
                Execute(commands, actuator);
                AfterPlay();
            }
        }

        private void Execute(IEnumerable<ICommand> commands, IActuator actuator)
        {
            if (commands != null)
            {
                foreach (var nextCommand in commands)
                {
                    nextCommand.Execute(actuator);
                }
            }
        }

        private bool MustPlay(GameState gameState)
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));

            if (gameState.Piece == null && timeNextAction <= clock.Time)
            {
                // await next tetromino when the piece was null some time in the past
                // and the timer to look for the next piece is exceeded
                awaitNextTetromino = true;
            }
            
            logger.Info("Current game state: " + ai.CurrentGameState);

            return awaitNextTetromino &&
                gameState.Piece != null &&
                gameState.NextPiece != null;
        }

        public void AfterPlay()
        {
            // start next timer
            var duration = TetrisLevel.GetFreeFallDuration(ai.LastWay.Fall);

            timeNextAction = clock.Time.Add(duration);
            awaitNextTetromino = false;
        }

        public IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);
            var tetrisExtractor = extractor as TetrisExtractor;
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
