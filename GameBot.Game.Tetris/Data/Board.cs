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
        private static int[] columnHeights;
        private static int[] columnHoles;
        private static int[] linePosition;

        private static void CalculateLookupTables()
        {
            columnHeights = new int[0x7FFFF + 1];
            columnHoles = new int[0x7FFFF + 1];
            linePosition = new int[0x7FFFF + 1];
            for (int i = 1; i < 0x7FFFF + 1; i++)
            {
                // calculate heights
                columnHeights[i] = 1 + (int)Math.Log(i, 2);

                // calculate holes
                int holes = 0;
                int tempCount = 0;
                for (int y = 0; y < 32; y++)
                {
                    if ((i & (1 << y)) > 0)
                    {
                        holes += tempCount;
                        tempCount = 0;
                    }
                    else
                    {
                        tempCount++;
                    }
                }
                columnHoles[i] = holes;

                // calculate line position
                linePosition[i] = -1;
                for (int y = 0; y < 32; y++)
                {
                    if ((i & (1 << y)) > 0)
                    {
                        linePosition[i] = y;
                        break;
                    }
                }
            }
        }

        private static int GetColumnHeight(int column)
        {
            if (columnHeights == null)
            {
                CalculateLookupTables();
            }
            return columnHeights[column];
        }

        private static int GetColumnHoles(int column)
        {
            if (columnHoles == null)
            {
                CalculateLookupTables();
            }
            return columnHoles[column];
        }

        private static int GetLinePosition(int columns)
        {
            if (linePosition == null)
            {
                CalculateLookupTables();
            }
            return linePosition[columns];
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Pieces { get; private set; }

        /// <summary>
        /// Every column is represented by an integer. Every bit represents a square. 1 true means occupied, 0 means free.
        /// The least significant bit is the square with the coordinate y = 0.
        /// This implementation allows fast lookups of the column height or other values relevant to the board heuristic.
        /// </summary>
        private int[] Columns { get; }

        protected bool this[int x, int y]
        {
            get
            {
                if (!SquareExists(x, y)) throw new ArgumentException(string.Format("square with coordinates {0}, {1} not in board", x, y));
                return (Columns[x] & (1 << y)) > 0;
            }
            set
            {
                if (!SquareExists(x, y)) throw new ArgumentException(string.Format("square with coordinates {0}, {1} not in board", x, y));
                if (value)
                {
                    // set one bit
                    Columns[x] |= (1 << y);
                }
                else
                {
                    // clear one bit
                    Columns[x] &= ~(1 << y);
                }
            }
        }

        public int CompletedLines
        {
            get
            {
                // AND every column
                int mask = ~0;
                for (int x = 0; x < Width; x++)
                {
                    mask &= Columns[x];
                    if (mask == 0) return 0; // no lines
                }

                // count bits on mask
                int count = 0;
                while (mask != 0)
                {
                    count++;
                    mask &= (mask - 1);
                }
                return count;
            }
        }

        public int MaximumHeight
        {
            get
            {
                // AND every column
                int mask = ~0;
                for (int x = 0; x < Width; x++)
                {
                    mask &= Columns[x];
                }
                return columnHeights[mask];
            }
        }

        public Board(int width, int height)
        {
            if (height > 30) throw new ArgumentException("height must be not greater than 30");

            Width = width;
            Height = height;
            Pieces = 0;
            Columns = new int[width];
        }

        public Board() : this(10, 19)
        {
        }

        public Board(Board board)
        {
            Width = board.Width;
            Height = board.Height;
            Pieces = board.Pieces;
            Columns = new int[board.Width];
            Array.Copy(board.Columns, Columns, board.Columns.Length);
        }

        public bool IsOccupied(int x, int y)
        {
            return this[x, y] == true;
        }

        public bool IsFree(int x, int y)
        {
            return this[x, y] == false;
        }

        public void Occupy(int x, int y)
        {
            this[x, y] = true;
        }

        public void Free(int x, int y)
        {
            this[x, y] = false;
        }

        public int ColumnHeight(int x)
        {
            if (x >= Width) throw new ArgumentException("x must be lower than the width of the board");

            return GetColumnHeight(Columns[x]);
        }

        public int ColumnHoles(int x)
        {
            if (x >= Width) throw new ArgumentException("x must be lower than the width of the board");

            return GetColumnHoles(Columns[x]);
        }

        public bool SquareExists(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void Place(Piece piece)
        {
            foreach (var block in piece.Shape.Body)
            {
                int positionX = Coordinates.PieceOrigin.X + piece.X + block.X;
                int positionY = Coordinates.PieceOrigin.Y + piece.Y + block.Y;
                if (IsOccupied(positionX, positionY)) throw new ArgumentException("Square is already occupied");

                Occupy(positionX, positionY);
            }
            Pieces++;
        }

        // TODO: merge with drop?
        public int RemoveLines()
        {
            int removed = 0;
            for (int i = 0; i < Height; i++) // TODO: replace with simple while-true-loop?
            {
                // AND every column
                int mask = ~0;
                for (int x = 0; x < Width; x++)
                {
                    mask &= Columns[x];
                    if (mask == 0) return removed; // no lines
                }

                int y = GetLinePosition(mask);
                removed++;
                CopySquaresDown(y);
            }
            return removed;
        }

        private void CopySquaresDown(int yCompleteLine)
        {
            for (int x = 0; x < Width; x++)
            {
                // clear square on completed line
                this[x, yCompleteLine] = false;

                // copy from above
                var maskTop = ~0 << (yCompleteLine + 1);
                var maskBottom = ~maskTop;
                var top = (maskTop & Columns[x]) >> 1;
                var bottom = maskBottom & Columns[x];
                Columns[x] = top | bottom;
            }
        }

        public bool Intersects(Piece piece)
        {
            foreach (var block in piece.Shape.Body)
            {
                int positionX = Coordinates.PieceOrigin.X + piece.X + block.X;
                int positionY = Coordinates.PieceOrigin.Y + piece.Y + block.Y;
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
            return false;
        }
        
        public int DropDistance(Piece piece)
        {
            int distance = int.MaxValue;
            
            foreach (var block in piece.Shape.Head)
            {
                var distanceTest = (Coordinates.PieceOrigin.Y + block.Y + piece.Y) - ColumnHeight(Coordinates.PieceOrigin.X + block.X + piece.X);
                distance = Math.Min(distance, distanceTest);
            }

            return distance;
        }

        public bool CanDrop(Piece piece)
        {
            return DropDistance(piece) >= 0;
        }
        
        public override int GetHashCode()
        {
            // TODO: implement
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            Board other = obj as Board;
            if (other != null)
            {
                // TODO: implement faster!
                return
                    Width == other.Width &&
                    Height == other.Height &&
                    Pieces == other.Pieces &&
                    Columns.SequenceEqual(other.Columns);
            }
            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Board(p:{0})\n", Pieces);
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
