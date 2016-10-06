using System;
using GameBot.Core;
using GameBot.Core.Data;
using System.Linq;
using System.Drawing;
using GameBot.Game.Tetris.Data;
using System.IO;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Extraction
{
    public class TetrisExtractor : IExtractor<TetrisGameState>
    {
        private static Point BoardTileOrigin = new Point(2, 0);
        private static Point CurrentTileOrigin = new Point(5, 0);
        private static Point PreviewTileOrigin = new Point(15, 13);

        private readonly IConfig config;
        public float BlockThreshold { get; set; }

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

            //var board = ExtractBoard();
            var board = new Board();
            var currentPiece = ExtractSpawnedPieceOrigin(screenshot);
            var nextPiece = ExtractNextPiece(screenshot);

            var gameState = new TetrisGameState(board, currentPiece, nextPiece);

            return gameState;
        }

        private bool IsBlock(byte mean)
        {
            const int TileMeanThreshold = 195; // optimum is between 185 and 195
            return mean < TileMeanThreshold;
        }

        // Tiles: x : 5 - 8, y : 0 - 2
        public Board ExtractBoard(IScreenshot screenshot)
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

        // Searches the piece only in the spawning area on top of the board
        // Searches only pieces with the orientation 0
        // Tiles: x : 5 - 8, y : 0 - 2
        // returns null, if the piece was not found in the spawning area
        public Piece ExtractSpawnedPieceOrigin(IScreenshot screenshot)
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

        // Searches the piece only in the spawning area on top of the board and a specifiec number of tiles below
        // Searches only pieces with the orientation 0
        // returns null, if the piece was not found in the spawning area or the specifiec number of tiles below
        public Piece ExtractSpawnedPiece(IScreenshot screenshot, int searchHeight)
        {
            if (searchHeight < 0)
                throw new ArgumentException("searchHeight must be positive.");

            for (int i = 0; i < searchHeight; i++)
            {
                ushort mask = 0;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        byte mean = screenshot.GetTileMean(CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y + searchHeight);
                        if (IsBlock(mean))
                        {
                            int index = 4 * (2 - y) + (x);
                            mask |= (ushort)(1 << index);
                            Rectangles.Add(new Point(CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y + searchHeight));
                        }
                    }
                }
                var piece = Piece.FromMask(mask);
                if (piece != null)
                {
                    piece.Fall(searchHeight);
                    return piece;
                }
            }

            // not found
            return null;
        }

        // Extracts a piece in a 4 by 4 search window at the spiecified coordinates
        public Piece ExtractPiece(IScreenshot screenshot, int x, int y)
        {
            throw new NotImplementedException();
        }

        // Confirms that the piece has moved or roteted according to the command
        public Piece ConfirmPieceMovement(IScreenshot screenshot, Piece lastPosition, ICommand lastCommand)
        {
            throw new NotImplementedException();
        }

        // Tiles: x : 14 - 17, y : 13 - 16 
        public Tetromino? ExtractNextPiece(IScreenshot screenshot)
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

        public PieceDelta FindPiece(IScreenshot screenshot, Piece last, Piece target)
        {
            var delta = new PieceDelta(last, target);

            // TODO: implement

            return delta;
        }
    }
}
