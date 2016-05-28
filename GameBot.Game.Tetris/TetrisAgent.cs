using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Agents;
using System.Drawing;
using System;
using GameBot.Core.Ui;

namespace GameBot.Game.Tetris
{
    public class TetrisAgent : AbstractAgent<TetrisGameState>
    {
        private readonly ITimeProvider timeProvider;
        private readonly IDebugger debugger;

        private bool awaitNextTetromino = true;
        private TimeSpan timeNextAction = TimeSpan.Zero;

        public TetrisAgent(IExtractor<TetrisGameState> extractor, IPlayer<TetrisGameState> player, ITimeProvider timeProvider, IDebugger debugger) : base(extractor, player)
        {
            this.timeProvider = timeProvider;
            this.debugger = debugger;
        }

        public override bool MustPlay(TetrisGameState gameState)
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));

            if (gameState.Piece == null && timeNextAction <= timeProvider.Time)
            {
                // await next tetromino when the piece was null some time in the past
                // and the timer to look for the next piece is exceeded
                awaitNextTetromino = true;
            }

            var tetrisPlayer = (TetrisPlayer)Player;
            debugger.WriteStatic(tetrisPlayer.CurrentGameState);

            return awaitNextTetromino &&
                gameState.Piece != null &&
                gameState.NextPiece != null;
        }

        public override void AfterPlay()
        {
            // start next timer
            var tetrisPlayer = (TetrisPlayer)Player;
            var duration = TetrisLevel.GetFreeFallDuration(tetrisPlayer.LastMove.Fall);

            timeNextAction = timeProvider.Time.Add(duration);
            awaitNextTetromino = false;
        }

        public override IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);
            var tetrisExtractor = Extractor as TetrisExtractor;
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
