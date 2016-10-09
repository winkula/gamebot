﻿using GameBot.Core.Data;
using GameBot.Core.Exceptions;
using System.Text;

namespace GameBot.Game.Tetris.Data
{
    public class GameState : AbstractGameState
    {
        public Board Board { get; private set; }
        public Piece Piece { get; set; }
        public Tetromino? NextPiece { get; set; }
        public Way Move { get; set; }
        public int Lines { get; set; }
        public int Score { get; set; }
        public int StartLevel { get; set; }
        public int Level { get { return TetrisLevel.GetLevel(StartLevel, Lines); } }

        public GameState()
        {
            Board = new Board();
            Piece = new Piece();
            NextPiece = Tetrominos.GetRandom(); // TODO: only generate if needed!
        }

        public GameState(GameState old)
        {
            Board = new Board(old.Board);
            if (old.Piece != null) Piece = new Piece(old.Piece);
            NextPiece = old.NextPiece;
            Lines += old.Lines;
            Score += old.Score;
        }

        public GameState(GameState old, Piece piece)
        {
            Board = new Board(old.Board);
            Piece = piece;
            NextPiece = old.NextPiece;
            Lines += old.Lines;
            Score += old.Score;
        }

        public GameState(Piece piece, Tetromino? nextPiece)
        {
            Board = new Board();
            Piece = piece;
            NextPiece = nextPiece;
        }

        public GameState(Board board, Piece piece, Tetromino? nextPiece)
        {
            Board = board;
            Piece = piece;
            NextPiece = nextPiece;
        }

        public GameState(Tetromino tetromino, Tetromino? nextTetromino) : this(new Piece(tetromino), nextTetromino)
        {
        }

        public GameState(Tetromino tetromino) : this(new Piece(tetromino), null)
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

        // returns the fallen distance of the piece
        public int Drop()
        {
            return Drop(Tetrominos.GetRandom());
        }

        // returns the fallen distance of the piece
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
            if (Board.Intersects(new Piece(Piece).Left()))
                throw new GameOverException("Left not possible");

            Piece.Left();
        }

        public void Right()
        {
            if (Board.Intersects(new Piece(Piece).Right()))
                throw new GameOverException("Right not possible");

            Piece.Right();            
        }

        public void Rotate()
        {
            if (Board.Intersects(new Piece(Piece).Rotate()))
                throw new GameOverException("Rotate not possible");

            Piece.Rotate();            
        }

        public void RotateCounterclockwise()
        {
            if (Board.Intersects(new Piece(Piece).RotateCounterclockwise()))
                throw new GameOverException("RotateCounterclockwise not possible");

            Piece.RotateCounterclockwise();
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
            GameState other = obj as GameState;
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
                    if (Piece != null && Piece.IsSquareOccupiedRegardTranslation(x - Coordinates.PieceOrigin.X, y - Coordinates.PieceOrigin.Y))
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
