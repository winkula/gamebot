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
    public class TetrisExtractor : IExtractor<GameState>
    {
        // this coordinates are in the coordinate system of the tile system of the game boy screen (origin is top left)
        private static Point BoardTileOrigin = new Point(2, 0);
        private static Point CurrentTileOrigin = new Point(5, 0);
        private static Point PreviewTileOrigin = new Point(15, 13);
        
        private readonly IConfig config;

        public float BlockThreshold { get; set; }

        // where is the threshold of the mean value of a tile, that it is interpreted as a block
        // optimum is between 185 and 195
        public static int MeanThreshold = 195;

        // debugging/visualization only
        public IList<Point> Rectangles { get; private set; } = new List<Point>();

        public TetrisExtractor(IConfig config)
        {
            this.config = config;
            this.BlockThreshold = config.Read<float>("Game.Tetris.Extractor.BlockThreshold");
        }

        public GameState Extract(IScreenshot screenshot, GameState currentGameState)
        {
            Rectangles.Clear();

            //var board = ExtractBoard();
            var board = new Board();
            var currentPiece = ExtractSpawnedPieceOrigin(screenshot);
            var nextPiece = ExtractNextPiece(screenshot);

            var gameState = new GameState(board, currentPiece, nextPiece);

            return gameState;
        }

        // this masks are built from the screen tiles like this (the number represents the bits from lowest to heighest):
        //
        // 13 14 15 16
        //  9 10 11 12
        //  5  6  7  8
        //  1  2  3  4
        //
        public ushort GetPieceMask(IScreenshot screenshot, int pieceX, int pieceY)
        {
            var tileCoordinates = Coordinates.PieceToSearchWindow(pieceX, pieceY);
            ushort mask = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (IsTileBlock(screenshot, tileCoordinates.X + x, tileCoordinates.Y + y))
                    {
                        int index = 4 * (3 - y) + (x);
                        mask |= (ushort)(1 << index);
                    }
                }
            }
            return mask;
        }

        public ushort GetPieceMask(IScreenshot screenshot, Piece pieceExpected)
        {
            return GetPieceMask(screenshot, pieceExpected.X, pieceExpected.Y);
        }

        public static bool IsBlock(byte mean)
        {
            return mean < MeanThreshold;
        }

        // x and y are in board coordinates
        private bool IsTileBlock(IScreenshot screenshot, int x, int y)
        {
            // TODO: make relative to board size?
            // ignore walls
            if (x < 2 || x > 11) return false;
            if (y < 0 || y > 17) return false;

            var mean = screenshot.GetTileMean(x, y);
            return IsBlock(mean);
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
                    if (IsTileBlock(screenshot, CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y))
                    {
                        int index = 4 * (2 - y) + (x);
                        mask |= (ushort)(1 << index);
                        Rectangles.Add(new Point(CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y));
                    }
                }
            }
            return Piece.FromMask(mask);
        }

        // TODO: fix this!
        // Searches the piece only in the spawning area on top of the board and a specifiec number of tiles below
        // Searches only pieces with the orientation 0
        // returns null, if the piece was not found in the spawning area or the specifiec number of tiles below
        public Piece ExtractSpawnedPiece(IScreenshot screenshot, int searchHeight)
        {
            if (searchHeight < 0)
                throw new ArgumentException("searchHeight must be positive.");

            for (int yDelta = 0; yDelta < searchHeight; yDelta++)
            {
                ushort mask = 0;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (IsTileBlock(screenshot, CurrentTileOrigin.X + x, CurrentTileOrigin.Y + y + yDelta))
                        {
                            int index = 4 * (2 - y) + (x);
                            mask |= (ushort)(1 << index);
                        }
                    }
                }
                var piece = Piece.FromMask(mask);
                if (piece != null)
                {
                    piece.Fall(yDelta);
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
        public Piece ExtractMovedPiece(IScreenshot screenshot, Piece lastPosition, Move move, int maxFallDistance)
        {
            if (maxFallDistance < 0)
                throw new ArgumentException("searchHeight must be positive.");

            if (move == Move.None) return lastPosition;

            var lastPositionTemp = new Piece(lastPosition);
            var expectedPosition = new Piece(lastPosition).Apply(move);

            // TODO: add error tolerance of the camera
            for (int i = 0; i <= maxFallDistance; i++)
            {
                var errorsLast = GetErrors(screenshot, lastPositionTemp);
                var errorsExpected = GetErrors(screenshot, expectedPosition);

                if (errorsLast != errorsExpected)
                {
                    if (errorsLast == 0)
                    {
                        // piece stayed in the lat position (did not move)
                        return lastPositionTemp;
                    }
                    if (errorsExpected == 0)
                    {
                        // piece was moved
                        return expectedPosition;
                    }
                }

                lastPositionTemp.Fall();
                expectedPosition.Fall();
            }

            // piece not found
            // TODO: search with more sophisticated search algorithms
            return null;
        }
        
        public int GetErrors(IScreenshot screenshot, Piece expected)
        {
            int errors = 0;
            foreach (var block in expected.Shape.Body)
            {
                var coordinates = Coordinates.PieceToTile(expected.X + block.X, expected.Y + block.Y);
                if (!IsTileBlock(screenshot, coordinates.X, coordinates.Y))
                {
                    errors++;
                }
            }

            if (errors > 0 && expected.Tetromino == Tetromino.I && expected.Y == 0 && expected.Orientation == 1)
            {
                // TODO: remove this ugly hack
                // subtract one because the I piece is not completly visible in upright position
                errors--;
            }

            return errors;
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
