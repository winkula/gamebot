using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using System.Drawing;
using System;
using GameBot.Core.Ui;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Agents
{
    public class PessimisticTetrisAgent : IAgent
    {
        private readonly ITimeProvider timeProvider;
        private readonly IDebugger debugger;
        private readonly IExtractor<TetrisGameState> extractor;
        private readonly TetrisAi ai;

        private bool initialized = false;
        private bool awaitNextTetromino = true;
        private TimeSpan timeNextAction = TimeSpan.Zero;

        private Queue<ICommand> commandQueue;
        private ICommand lastCommand;
        private TetrisGameState goalState;
        
        public PessimisticTetrisAgent(IExtractor<TetrisGameState> extractor, TetrisAi ai, ITimeProvider timeProvider, IDebugger debugger)
        {
            this.timeProvider = timeProvider;
            this.debugger = debugger;
            this.extractor = extractor;
            this.ai = ai;
            this.commandQueue = new Queue<ICommand>();
        }
              
        public void Act(IScreenshot screenshot, IActuator actuator)
        {
            var gameState = extractor.Extract(screenshot, ai.CurrentGameState);

            if (commandQueue.Any())
            {
                // there are commands to execute
                
                if (lastCommand != null)
                {
                    // check if last command was executed
                    // if not, repeat                                
                }

                var command = commandQueue.Dequeue();
                if (command != null)
                {
                    command.Execute(actuator);
                    lastCommand = command;
                }
            }
            else
            {
                // all commands are executed

                if (!initialized)
                {
                    initialized = true;
                    var commands = ai.Initialize();
                    foreach (var command in commands)
                    {
                        commandQueue.Enqueue(command);
                    }
                }
                else if (MustPlay(gameState))
                {
                    var commands = ai.Play(gameState);
                    foreach (var command in commands)
                    {
                        commandQueue.Enqueue(command);
                    }
                    AfterPlay();
                }
            }   
        }

        private bool MustPlay(TetrisGameState gameState)
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));

            if (gameState.Piece == null && timeNextAction <= timeProvider.Time)
            {
                // await next tetromino when the piece was null some time in the past
                // and the timer to look for the next piece is exceeded
                awaitNextTetromino = true;
            }
            
            debugger.WriteStatic(ai.CurrentGameState);

            return awaitNextTetromino &&
                gameState.Piece != null &&
                gameState.NextPiece != null;
        }

        public void AfterPlay()
        {
            // start next timer
            var duration = TetrisLevel.GetFreeFallDuration(ai.LastMove.Fall);

            timeNextAction = timeProvider.Time.Add(duration);
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
    }
}
