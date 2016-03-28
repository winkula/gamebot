using System;
using GameBot.Core;
using GameBot.Core.Data;
using System.Linq;
using System.Drawing;
using GameBot.Game.Tetris.Data;
using System.IO;

namespace GameBot.Game.Tetris
{
    public class TetrisExtractor : IExtractor<TetrisGameStateFull>
    {
        private static Point BoardTileOrigin = new Point(2, 0);
        private static Point CurrentTileOrigin = new Point(5, 0);
        private static Point PreviewTileOrigin = new Point(15, 13);

        private IScreenshot screenshot;

        public TetrisGameStateFull Extract(IScreenshot screenshot, IContext<TetrisGameStateFull> context)
        {
            this.screenshot = screenshot;

            var gameState = new TetrisGameStateFull();

            var board = ExtractBoard();
            var currentPiece = ExtractCurrentPiece();
            var nextPiece = ExtractNextPiece();

            gameState.State = new TetrisGameState(board, currentPiece, nextPiece);

            if (gameState.State.Piece != null && gameState.State.NextPiece != null)
            {
                string text = string.Format("{0}\n", gameState.State.Piece.Tetromino);
                File.AppendAllText(string.Format(@"{0}\tetris_extractor_log.txt", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), text);
            }

            return gameState;
        }

        // TODO: better threshold
        private bool TileThresholdReached(int value)
        {
            return value > 64;
        }

        // Tiles: x : 5 - 8, y : 0 - 2
        private Board ExtractBoard()
        {
            var board = new Board();
            for (int x = 0; x < 10; x++)
            {
                // TODO: extract whole board but subtract current piece
                for (int y = 3; y < 18; y++)
                {
                    int sum = screenshot.GetTile(BoardTileOrigin.X + x, BoardTileOrigin.Y + y).Sum();
                    if (TileThresholdReached(sum))
                    {
                        board.Occupy(x, 17 - y);
                    }
                }
            }
            return board;
        }

        // Tiles: x : 5 - 8, y : 0 - 2
        private Piece ExtractCurrentPiece()
        {
            ushort mask = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    int sum = screenshot.GetTile(CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y).Sum();
                    if (TileThresholdReached(sum))
                    {
                        int index = 4 * (2 - y) + (x);
                        mask |= (ushort)(1 << index);
                    }
                }
            }
            return Piece.FromMask(mask);
        }

        // Tiles: x : 14 - 17, y : 13 - 16 
        private Piece ExtractNextPiece()
        {
            ushort mask = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    int sum = screenshot.GetTile(PreviewTileOrigin.X + x, PreviewTileOrigin.Y + y).Sum();
                    if (TileThresholdReached(sum))
                    {
                        int index = 4 * (2 - y) + (x);
                        mask |= (ushort)(1 << index);
                    }
                }
            }
            return Piece.FromMask(mask);
        }
    }
}
