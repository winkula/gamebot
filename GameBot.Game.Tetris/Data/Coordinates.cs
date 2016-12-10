﻿using System.Drawing;

namespace GameBot.Game.Tetris.Data
{
    /// <summary>
    /// There are three different coordinate systems that are relevant for the a.i.:
    /// - The board coordinate system
    ///     The origin is bottom left of the board
    /// - The piece coordinate system 
    ///     The origin is in the piece spawn position on the board, at (4, 16) 
    /// - The tile coordinate system
    ///     The origin is top left on the game boy screen)
    ///     Every tile is a 8 by 8 pixel block
    /// </summary>
    public static class Coordinates
    {
        // origin of spawning pieces (center of the piece)
        // in absolute game coordinates (origin in bottom left of the board)
        public static Point PieceOrigin = new Point(4, 16);
        public static Point NextPieceTileOrigin = new Point(15, 13);

        #region To board coordinate system
        
        public static Point PieceToBoard(int x, int y)
        {
            return new Point(x + PieceOrigin.X, y + PieceOrigin.Y);
        }

        public static Point PieceToBoard(Point coordinates)
        {
            return PieceToBoard(coordinates.X, coordinates.Y);
        }

        #endregion

        #region To piece coordinate system

        public static Point BoardToPiece(int x, int y)
        {
            return new Point(x - PieceOrigin.X, y - PieceOrigin.Y);
        }

        public static Point BoardToPiece(Point coordinates)
        {
            return BoardToPiece(coordinates.X, coordinates.Y);
        }

        #endregion

        #region To tile coordinate system

        public static Point BoardToTile(int x, int y)
        {
            return new Point(x + 2, -y + 17);
        }

        public static Point BoardToTile(Point coordinates)
        {
            return BoardToTile(coordinates.X, coordinates.Y);
        }

        public static Point PieceToTile(int x, int y)
        {
            return new Point(x + 6, 1 - y);
        }

        public static Point PieceToTile(Point coordinates)
        {
            return PieceToTile(coordinates.X, coordinates.Y);
        }
        
        // converts game coordinates (X and Y of Piece) to the origin (top left) of the search window (4 by 4)
        // this is used for creating bitmasks for piece matching        
        public static Point PieceToTileSearchWindowOrigin(int x, int y)
        {
            return new Point(x + 5, -y - 1);
        }

        public static Point PieceToTileSearchWindowOrigin(Point coordinates)
        {
            return PieceToTileSearchWindowOrigin(coordinates.X, coordinates.Y);
        }
        
        public static Point PieceToTilePreview(int x, int y)
        {
            return new Point(x + 5 + 11, -y - 1 + 15);
        }

        public static Point PieceToTilePreview(Point coordinates)
        {
            return PieceToTilePreview(coordinates.X, coordinates.Y);
        }

        #endregion
    }
}
