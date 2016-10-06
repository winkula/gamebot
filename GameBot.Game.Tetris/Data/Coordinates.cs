using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Game.Tetris.Data
{
    public class Coordinates
    {
        // origin of spawning pieces (center of the piece)
        // in absolute game coordinates (origin in bottom left of the board)
        public static Point PieceOrigin = new Point(4, 16);

        // converts game coordinates (origin is bottom left of the board)
        // to tile (8 by 8 pixel blocks) coordinates (origin is top left of the game boy screen)
        public static Point GameToTile(int x, int y)
        {
            return new Point(x + 2, -y + 17);
        }

        public static Point GameToTile(Point coordinates)
        {
            return GameToTile(coordinates.X, coordinates.Y);
        }




        
        //public static Point GameToTile(Point coordinates)
        //{
        //    return new Point(coordinates.X + 5, -coordinates.Y + 1);
        //}

        //public static Point GameToTile(int x, int y)
        //{
        //    return new Point(x + 5, -y + 1);
        //}

        // converts game coordinates (X and Y of Piece) to the origin (top left) of the search window (4 by 4)
        // this is used for creating bitmasks for piece matching
        public static Point GameToSearchWindow(Point coordinates)
        {
            return new Point(coordinates.X + 4, -coordinates.Y - 1);
        }

        public static Point GameToSearchWindow(int x, int y)
        {
            return new Point(x + 4, -y - 1);
        }

        // converts tile coordinates (tiles are 8 by 8 pixel blocks on the screen of the game boy) to game coordinates (X and Y of Piece)
        public static Point TileToGame(Point coordinates)
        {
            return new Point(coordinates.X - 5, -coordinates.Y + 1);
        }

        public static Point TileToGame(int x, int y)
        {
            return new Point(x - 5, -y + 1);
        }
    }
}
