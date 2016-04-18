﻿using GameBot.Core.Data;
using GameBot.Core.Exceptions;
using GameBot.Game.Tetris.Data;
using System;
using System.Text;

namespace GameBot.Game.Tetris
{
    public class TetrisGameState : AbstractGameState
    {
        public Board Board { get; private set; }
        public Piece Piece { get; set; }
        public Tetromino? NextPiece { get; set; }
        public Move Move { get; set; }
        public int Lines { get; set; }
        public int Score { get; set; }
        public int Level { get { return TetrisLevel.GetLevel(0, Lines); } }

        public TetrisGameState()
        {
            Board = new Board();
            Piece = new Piece();
            NextPiece = Tetrominos.GetRandom(); // TODO: only generate if needed!
        }

        public TetrisGameState(TetrisGameState old)
        {
            Board = new Board(old.Board);
            if (old.Piece != null) Piece = new Piece(old.Piece);
            NextPiece = old.NextPiece;
            Lines += old.Lines;
            Score += old.Score;
        }

        public TetrisGameState(TetrisGameState old, Piece piece)
        {
            Board = new Board(old.Board);
            Piece = piece;
            NextPiece = old.NextPiece;
            Lines += old.Lines;
            Score += old.Score;
        }

        public TetrisGameState(Piece piece, Tetromino? nextPiece)
        {
            Board = new Board();
            Piece = piece;
            NextPiece = nextPiece;
        }

        public TetrisGameState(Board board, Piece piece, Tetromino? nextPiece)
        {
            Board = board;
            Piece = piece;
            NextPiece = nextPiece;
        }

        public TetrisGameState(Tetromino tetromino, Tetromino? nextTetromino) : this(new Piece(tetromino), nextTetromino)
        {
        }

        public TetrisGameState(Tetromino tetromino) : this(new Piece(tetromino), null)
        {
        }

        public bool IsPieceLanded
        {
            get { return Board.Intersects(new Piece(Piece).Fall()); }
        }

        public bool Fall()
        {
            return Fall(Tetrominos.GetRandom());
        }

        public bool Fall(Tetromino next)
        {
            if (Board.DropDistance(Piece) < 0 && Board.Intersects(Piece)) throw new GameOverException();

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

                if (NextPiece.HasValue)
                {
                    Piece = new Piece(NextPiece.Value);
                    NextPiece = next;
                }
            }

            return fallen;
        }

        public int Drop()
        {
            return Drop(Tetrominos.GetRandom());
        }

        public int Drop(Tetromino next)
        {
            int distance = Board.DropDistance(Piece);
            if (distance < 0 && Board.Intersects(Piece)) throw new GameOverException();

            // let piece fall
            Piece.Fall(distance);
            Board.Place(Piece);
            Piece = null;

            // remove lines
            int lines = Board.RemoveLines();
            Lines += lines;

            // calculate score
            Score += TetrisScore.GetSoftdropScore(distance);
            Score += TetrisScore.GetLineScore(lines, Level);

            if (NextPiece.HasValue)
            {
                // TODO: only generate new piece if this is explicitly wanted!
                Piece = new Piece(NextPiece.Value);
                NextPiece = next;
            }

            return distance;
        }

        public void Left()
        {
            if (!Board.Intersects(new Piece(Piece).Left()))
            {
                Piece.Left();
            }
        }

        public void Right()
        {
            if (!Board.Intersects(new Piece(Piece).Right()))
            {
                Piece.Right();
            }
        }

        public void Rotate()
        {
            if (!Board.Intersects(new Piece(Piece).Rotate()))
            {
                Piece.Rotate();
            }
        }

        public void RotateCounterclockwise()
        {
            if (!Board.Intersects(new Piece(Piece).RotateCounterclockwise()))
            {
                Piece.RotateCounterclockwise();
            }
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
