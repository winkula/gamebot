﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameBot.Game.Tetris.Data
{
    /// <summary>
    /// The origin of the board is bottom left.
    /// </summary>
    public class Board
    {
        public static Point Origin = new Point(4, 16);

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Pieces { get; private set; }

        /// <summary>
        /// true means occupied, false means free.
        /// </summary>
        private bool[] squares;
        
        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            Pieces = 0;
            squares = new bool[width * height];
        }

        public Board() : this(10, 19)
        {
        }

        public Board(Board board)
        {
            Width = board.Width;
            Height = board.Height;
            Pieces = board.Pieces;
            squares = new bool[board.Width * board.Height];
            Array.Copy(board.squares, squares, board.squares.Length);
        }

        public bool IsOccupied(int x, int y)
        {
            if (!SquareExists(x, y)) throw new ArgumentException(string.Format("square with coordinates {0}, {1} not in board", x, y));
            return squares[Width * y + x];
        }

        public void Occupy(int x, int y)
        {
            if (!SquareExists(x, y)) throw new ArgumentException(string.Format("square with coordinates {0}, {1} not in board", x, y));
            squares[Width * y + x] = true;
        }

        public bool SquareExists(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void Apply(Piece piece)
        {
            // TODO: use constants
            for (int x = Piece.CoordinateMin; x <= Piece.CoordinateMax; x++)
            {
                for (int y = Piece.CoordinateMin; y <= Piece.CoordinateMax; y++)
                {
                    bool occupied = piece.IsSquareOccupied(x, y);
                    if (occupied)
                    {
                        int positionX = Origin.X + piece.X + x;
                        int positionY = Origin.Y + piece.Y + y;
                        Occupy(positionX, positionY);

                        // TODO: remove
                        /*
                        if (SquareExists(positionX, positionY))
                        {
                            SetSquare(positionX, positionY);
                        }*/
                    }
                }
            }
            Pieces++;
        }

        public bool Intersects(Piece piece)
        {
            for (int x = Piece.CoordinateMin; x <= Piece.CoordinateMax; x++)
            {
                for (int y = Piece.CoordinateMin; y <= Piece.CoordinateMax; y++)
                {
                    bool occupied = piece.IsSquareOccupied(x, y);
                    if (occupied)
                    {
                        int positionX = Origin.X + piece.X + x;
                        int positionY = Origin.Y + piece.Y + y;
                        if (SquareExists(positionX, positionY))
                        {
                            if (IsOccupied(positionX, positionY))
                            {
                                // intersection with already placed pieces
                                return true;
                            }
                        }
                        else
                        {
                            // intersection with border of the board
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Pieces: {0}\n", Pieces);
            builder.Append(" ");
            builder.AppendLine(new string('-', Width));
            for (int y = Height - 1; y >= 0; y--)
            {
                builder.Append("|");
                for (int x = 0; x < Width; x++)
                {
                    if (IsOccupied(x, y)) builder.Append('#');
                    else builder.Append(' ');
                }
                builder.AppendLine("|");
            }
            builder.Append(" ");
            builder.Append(new string('-', Width));
            return builder.ToString();
        }
    }
}
