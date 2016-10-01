using System;
using GameBot.Core;
using GameBot.Core.Data;
using System.Linq;
using System.Drawing;
using GameBot.Game.Tetris.Data;
using System.IO;
using System.Collections.Generic;

namespace GameBot.Game.Tetris
{
    public class TetrisExtractor : IExtractor<TetrisGameState>
    {
        private static Point BoardTileOrigin = new Point(2, 0);
        private static Point CurrentTileOrigin = new Point(5, 0);
        private static Point PreviewTileOrigin = new Point(15, 13);

        private readonly IConfig config;
        public float BlockThreshold { get; set; }
        private IScreenshot screenshot;

        // debugging/visualization only
        public IList<Point> Rectangles { get; private set; } = new List<Point>();

        public TetrisExtractor(IConfig config)
        {
            this.config = config;
            this.BlockThreshold = config.Read<float>("Game.Tetris.Extractor.BlockThreshold");
        }

        public TetrisGameState Extract(IScreenshot screenshot, TetrisGameState currentGameState)
        {
            Rectangles.Clear();

            this.screenshot = screenshot;

            //var board = ExtractBoard();
            var board = new Board();
            var currentPiece = ExtractCurrentPiece();
            var nextPiece = ExtractNextPiece();

            var gameState = new TetrisGameState(board, currentPiece, nextPiece);

            return gameState;
        }
        
        private bool IsBlock(byte mean)
        {
            const int TileMeanThreshold = 195; // optimum is between 185 and 195
            return mean < TileMeanThreshold;     
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
                    byte mean = screenshot.GetTileMean(BoardTileOrigin.X + x, BoardTileOrigin.Y + y);
                    if (IsBlock(mean))
                    {
                        board.Occupy(x, 17 - y);
                        Rectangles.Add(new Point(BoardTileOrigin.X + x, BoardTileOrigin.Y + y));
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
                    byte mean = screenshot.GetTileMean(CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y);
                    if (IsBlock(mean))
                    {
                        int index = 4 * (2 - y) + (x);
                        mask |= (ushort)(1 << index);
                        Rectangles.Add(new Point(CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y));
                    }
                }
            }
            return Piece.FromMask(mask);
        }

        // Tiles: x : 14 - 17, y : 13 - 16 
        private Tetromino? ExtractNextPiece()
        {
            ushort mask = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    byte mean = screenshot.GetTileMean(PreviewTileOrigin.X + x, PreviewTileOrigin.Y + y);
                    if (IsBlock(mean))
                    {
                        int index = 4 * (2 - y) + (x);
                        mask |= (ushort)(1 << index);
                        Rectangles.Add(new Point(PreviewTileOrigin.X + x, PreviewTileOrigin.Y + y));
                    }
                }
            }
            return Piece.FromMask(mask)?.Tetromino;
        }
    }
}
