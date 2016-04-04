using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameBot.Game.Tetris
{
    public class TetrisGameState : AbstractGameState
    {
        public Board Board { get; private set; }
        public Piece Piece { get; set; }
        public Piece NextPiece { get; set; }
        public Move Move { get; set; }
        public int Lines { get; set; }
        public int Level { get { return Tetris.Level.GetLevelAType(0, Lines); } }

        //public int? Player { get; set; }
        //public GameType? GameType { get; set; }
        //public int? MusicType { get; set; }
        //public int? Level
        //{
        //    get { return State?.Board.CompletedLines; }
        //}
        //public int? High { get; set; }
        //public int? Score { get; set; }
        //public bool? IsPause { get; set; }

        public TetrisGameState()
        {
            Board = new Board();
            Piece = new Piece();
            NextPiece = new Piece();
        }

        public TetrisGameState(TetrisGameState old)
        {
            Board = new Board(old.Board);
            if (old.Piece != null) Piece = new Piece(old.Piece);
            if (old.NextPiece != null) NextPiece = new Piece(old.NextPiece);
            Lines += old.Lines;
        }

        public TetrisGameState(TetrisGameState old, Piece piece)
        {
            Board = new Board(old.Board);
            Piece = piece;
            if (old.NextPiece != null) NextPiece = new Piece(old.NextPiece);
            Lines += old.Lines;
        }

        public TetrisGameState(Piece piece, Piece nextPiece)
        {
            Board = new Board();
            Piece = piece;
            NextPiece = nextPiece;
        }

        public TetrisGameState(Board board, Piece piece, Piece nextPiece)
        {
            Board = board;
            Piece = piece;
            NextPiece = nextPiece;
        }

        public TetrisGameState(Tetromino tetromino, Tetromino nextTetromino) : this(new Piece(tetromino), new Piece(nextTetromino))
        {
        }

        public TetrisGameState(Tetromino tetromino) : this(new Piece(tetromino), null)
        {
        }

        public bool IsEnd
        {
            get { return Board.Intersects(NextPiece); }
        }

        public bool IsPieceLanded
        {
            get { return Board.Intersects(new Piece(Piece).Fall()); }
        }

        public bool Fall(Piece next = null)
        {
            bool fallen = false;

            if (!IsPieceLanded)
            {
                Piece.Fall();
                fallen = true;
            }

            if (IsPieceLanded)
            {
                Board.Place(Piece);
                Piece = null;

                if (NextPiece != null)
                {
                    Piece = new Piece(NextPiece);
                    NextPiece = next ?? new Piece();
                }
            }

            return fallen;
        }

        public int Drop(Piece next = null)
        {
            int distance = Board.DropDistance(Piece);
            if (distance < 0) throw new ArgumentException("Piece can't be dropped");

            // let piece fall
            Piece.Fall(distance);
            Board.Place(Piece);
            Piece = null;

            // remove lines
            int lines = Board.RemoveLines();
            Lines += lines;

            if (NextPiece != null)
            {
                // TODO: only generate new pice if this is explicitly wanted!
                Piece = new Piece(NextPiece);
                NextPiece = next ?? new Piece();
            }

            return distance;
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
            TetrisGameState other = obj as TetrisGameState;
            if (other != null)
            {
                return
                    ((Board == null && other.Board == null) || (Board != null && Board.Equals(other.Board))) &&
                    ((Piece == null && other.Piece == null) || (Piece != null && Piece.Equals(other.Piece))) &&
                    ((NextPiece == null && other.NextPiece == null) || (NextPiece != null && NextPiece.Equals(other.NextPiece)));
            }
            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(" ");
            builder.AppendLine(new string('-', Board.Width));
            for (int y = Board.Height - 1; y >= 0; y--)
            {
                builder.Append("|");
                for (int x = 0; x < Board.Width; x++)
                {
                    if (Piece.IsSquareOccupiedRegardTranslation(x - Board.Origin.X, y - Board.Origin.Y))
                    {
                        builder.Append('*');
                    }
                    else
                    {
                        if (Board.IsOccupied(x, y)) builder.Append('#');
                        else builder.Append(' ');
                    }
                }
                builder.AppendLine("|");
            }
            builder.Append(" ");
            builder.Append(new string('-', Board.Width));
            return builder.ToString();
        }
    }
}
