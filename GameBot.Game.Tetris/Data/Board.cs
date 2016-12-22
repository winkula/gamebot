using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GameBot.Game.Tetris.Data
{
    /// <summary>
    /// The origin of the board is bottom left.
    /// </summary>
    public class Board
    {
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Number of placed pieces on the board.
        /// </summary>
        public int Pieces { get; private set; }

        /// <summary>
        /// Every column is represented by an integer. Every bit represents a square. 1 true means occupied, 0 means free.
        /// The least significant bit is the square with the coordinate y = 0.
        /// This implementation allows fast lookups of the column height or other values relevant to the board heuristic.
        /// </summary>
        private int[] Columns { get; }

        private bool this[int x, int y]
        {
            get
            {
                return (Columns[x] & (1 << y)) > 0;
            }
            set
            {
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
                // OR every column
                int mask = 0;
                for (int x = 0; x < Width; x++)
                {
                    mask |= Columns[x];
                }
                return BoardLookups.Instance.GetColumnHeight(mask);
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

        public Board() : this(TetrisConstants.DefaultBoardWidth, TetrisConstants.DefaultBoardHeight)
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

        public Board Random(Random random = null)
        {
            const double chanceForBlock = 0.8;
            const double chanceToBreak = 0.3;
            random = random ?? new Random();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (random.NextDouble() < chanceForBlock)
                    {
                        Occupy(x, y);
                    }
                    if (random.NextDouble() < (chanceToBreak + 0.05 * y))
                    {
                        break;
                    }
                }
            }

            return this;
        }

        public bool IsOccupied(int x, int y)
        {
            if (!SquareExists(x, y)) throw new ArgumentException($"square with coordinates {x}, {y} not in board");

            return this[x, y];
        }

        public bool IsFree(int x, int y)
        {
            if (!SquareExists(x, y)) throw new ArgumentException($"square with coordinates {x}, {y} not in board");

            return this[x, y] == false;
        }

        public void Occupy(int x, int y)
        {
            if (!SquareExists(x, y)) throw new ArgumentException($"square with coordinates {x}, {y} not in board");

            this[x, y] = true;
        }

        public void Free(int x, int y)
        {
            if (!SquareExists(x, y)) throw new ArgumentException($"square with coordinates {x}, {y} not in board");

            this[x, y] = false;
        }

        public int ColumnHeight(int x)
        {
            if (x >= Width) throw new ArgumentException("x must be lower than the width of the board");

            return BoardLookups.Instance.GetColumnHeight(Columns[x]);
        }
        
        public int ColumnHeightUnchecked(int x)
        {
            return BoardLookups.Instance.GetColumnHeight(Columns[x]);
        }

        public int ColumnHoles(int x)
        {
            if (x >= Width) throw new ArgumentException("x must be lower than the width of the board");

            return BoardLookups.Instance.GetColumnHoles(Columns[x]);
        }
        
        public int ColumnHolesUnchecked(int x)
        {
            return BoardLookups.Instance.GetColumnHoles(Columns[x]);
        }

        public void FillColumn(int x, int height)
        {
            if (x < 0 || x >= Width) throw new ArgumentException();
            if (height < 0 || height > Height) throw new ArgumentException();

            Columns[x] = ~(~0 << height);
        }

        public bool SquareExists(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        // this should be private to guarantee consistent game state
        // is public for testing purposes
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

        public void PlaceUnchecked(Piece piece)
        {
            foreach (var block in piece.Shape.Body)
            {
                int positionX = Coordinates.PieceOrigin.X + piece.X + block.X;
                int positionY = Coordinates.PieceOrigin.Y + piece.Y + block.Y;
                this[positionX, positionY] = true; // occupy unchecked
            }
            Pieces++;
        }

        // this should be private to guarantee consistent game state
        // is public for testing purposes
        public int RemoveLines()
        {
            int removed = 0;
            for (int i = 0; i < Height; i++)
            {
                // AND every column
                int mask = ~0;
                for (int x = 0; x < Width; x++)
                {
                    mask &= Columns[x];
                    if (mask == 0) return removed; // no lines
                }

                int y = BoardLookups.Instance.GetLinePosition(mask);
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
                if (!SquareExists(x, yCompleteLine)) throw new ArgumentException($"square with coordinates {x}, {yCompleteLine} not in board");
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

        public int MaximumDropDistanceForSpawnedPiece()
        {
            int distance = 0;

            foreach (var tetromino in Tetriminos.All)
            {
                var distanceTest = DropDistance(new Piece(tetromino));
                distance = Math.Max(distance, distanceTest);
            }

            return distance;
        }

        public bool CanDrop(Piece piece)
        {
            return DropDistance(piece) >= 0;
        }

        // this is used in multiplayer mode
        internal void SpawnLines(int numLines, int holePosition)
        {
            if (numLines < 0 || numLines > 4) throw new ArgumentException("numLines must be between 0 and 4");
            if (holePosition < 0 || holePosition >= Width) throw new ArgumentException("holePosition must be a valid x coordinate on the board");

            if (numLines > 0)
            {
                for (int x = 0; x < Width; x++)
                {
                    // move board up
                    Columns[x] <<= numLines;

                    if (x != holePosition)
                    {
                        // insert new lines
                        int r = int.MaxValue >> (31 - numLines);
                        Columns[x] |= r;
                    }
                }
            }
        }

        public IEnumerable<Point> GetHorizon(int height)
        {
            if (height < 0) throw new ArgumentException("height must be non negative");

            for (int x = 0; x < Width; x++)
            {
                var columnHeight = ColumnHeight(x);
                yield return new Point(x, columnHeight + height);
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int x = 0; x < Width; x++)
            {
                hashCode ^= Columns[x] << (3 * x);
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            Board other = obj as Board;
            if (other != null)
            {
                return
                    Width == other.Width &&
                    Height == other.Height &&
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
                    builder.Append(IsOccupied(x, y) ? '#' : ' ');
                }
                builder.AppendLine("|");
            }
            builder.Append(" ");
            builder.Append(new string('-', Width));
            return builder.ToString();
        }
    }
}
