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
        // converts game coordinates (X and Y of Piece) to tile coordinates (tiles are 8 by 8 pixel blocks on the screen of the game boy)
        public static Point GameToTile(Point coordinates)
        {
            return new Point(coordinates.X + 5, -coordinates.Y + 1);
        }

        public static Point GameToTile(int x, int y)
        {
            return new Point(x + 5, -y + 1);
        }

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
