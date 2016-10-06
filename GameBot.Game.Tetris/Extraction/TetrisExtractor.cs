﻿using System;
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
        // this coordinates are in the coordinate system of the tile system of the game boy screen (origin is top left)
        private static Point BoardTileOrigin = new Point(2, 0);
        private static Point CurrentTileOrigin = new Point(5, 0);
        private static Point PreviewTileOrigin = new Point(15, 13);

        private static PieceMatcher PieceMatcher = PieceMatcher.Instance;

        private readonly IConfig config;

        // TODO: change this to a mean threshold of the mean value
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

        // this masks are built from the screen tiles like this (the number represents the bits from lowest to heighest):
        //
        // 13 14 15 16
        //  9 10 11 12
        //  5  6  7  8
        //  1  2  3  4
        //
        public ushort GetPieceMask(IScreenshot screenshot, int xGame, int yGame)
        {
            var tileCoordinates = Coordinates.GameToSearchWindow(xGame, yGame);
            ushort mask = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    byte mean = screenshot.GetTileMean(tileCoordinates.X + x, tileCoordinates.Y + y);
                    if (IsBlock(mean))
                    {
                        int index = 4 * (3 - y) + (x);
                        mask |= (ushort)(1 << index);
                    }
                }
            }
            return mask;
        }

        private bool IsBlock(byte mean)
        {
            // TODO: put this value in the config
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
        public Piece ConfirmPieceMove(IScreenshot screenshot, Piece lastPosition, Move move, int searchHeight)
        {
            if (searchHeight < 0)
                throw new ArgumentException("searchHeight must be positive.");

            if (move == Move.None) return lastPosition;
            
            var expectedPosition = new Piece(lastPosition);

            for (int i = 0; i < searchHeight; i++)
            {
                //PieceMatcher.GetMask(screenshot, )
            }

            return null;
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
