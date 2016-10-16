using System.Drawing;

namespace GameBot.Game.Tetris
{
    public static class TetrisConstants
    {
        // this coordinates are in the coordinate system of the tile system of the game boy screen (origin is top left)
        public static Point CurrentTileOrigin = new Point(5, 0);
        public static Point NextPieceTileOrigin = new Point(15, 13);
        public static Point NextPieceTemplateTileCoordinates = new Point(10, -13);
    }
}
