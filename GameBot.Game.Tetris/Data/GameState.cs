﻿using System;
using System.Collections.Generic;
using GameBot.Core.Exceptions;
using System.Text;

namespace GameBot.Game.Tetris.Data
{
    public class GameState
    {
        public Board Board { get; set; }
        public Piece Piece { get; set; }
        public Tetrimino? NextPiece { get; set; }

        internal Stack<Piece> Pieces = new Stack<Piece>();
        
        public int Lines { get; private set; }

        // max score is 999'999
        private int _score;
        public int Score
        {
            get
            {
                return _score;
            }
            private set
            {
                _score = Math.Min(999999, value);
            }
        }

        private int _startLevel;
        public int StartLevel
        {
            get
            {
                return _startLevel;
            }
            set
            {
                if (value < 0 || value > 9) throw new ArgumentException("StartLevel must be between 0 and 9.");
                _startLevel = value;
            }
        }

        public bool HeartMode { get; set; }

        public int Level => TetrisLevel.GetLevel(StartLevel, Lines);

        public GameState(GameState old)
        {
            Board = new Board(old.Board);
            if (old.Piece != null) Piece = new Piece(old.Piece);
            NextPiece = old.NextPiece;
            Lines += old.Lines;
            Score += old.Score;
            StartLevel = old.StartLevel;
        }

        // this constructor is only used in the search
        internal GameState(GameState old, Piece piece)
        {
            Board = new Board(old.Board);
            Piece = piece;
            NextPiece = old.NextPiece;
            Lines += old.Lines;
            Score += old.Score;
            StartLevel = old.StartLevel;
            Pieces = new Stack<Piece>(old.Pieces);
        }

        public GameState(Piece piece, Tetrimino? nextPiece)
        {
            Board = new Board();
            Piece = piece;
            NextPiece = nextPiece;
        }

        public GameState(Board board, Tetrimino current, Tetrimino? nextPiece = null)
        {
            Board = board ?? new Board();
            Piece = new Piece(current);
            NextPiece = nextPiece;
        }

        public GameState(Board board = null, Piece piece = null, Tetrimino? nextPiece = null)
        {
            Board = board ?? new Board();
            Piece = piece ?? new Piece();
            NextPiece = nextPiece;
        }

        public GameState(Tetrimino tetrimino, Tetrimino? nextTetrimino = null) : this(new Piece(tetrimino), nextTetrimino)
        {
        }

        // TODO: use drop distance here?
        private bool IsPieceLanded => Board.Intersects(new Piece(Piece).Fall());
        
        public bool FallAndLand()
        {
            return FallAndLand(Tetriminos.GetRandom());
        }

        public bool FallAndLand(Tetrimino next)
        {
            // TODO: check this again!
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

        public bool FallAndLand(int distance)
        {
            return FallAndLand(distance, Tetriminos.GetRandom());
        }

        public bool FallAndLand(int distance, Tetrimino next)
        {
            if (distance < 0) throw new ArgumentException("distance can't be negative");

            var dropDistance = Board.DropDistance(Piece);
            if (dropDistance < 0) throw new GameOverException();
            distance = Math.Min(distance, dropDistance);

            bool fallen = distance > 0;

            Piece.Fall(distance);
            dropDistance -= distance;

            if (dropDistance == 0)
            {
                // piece is landed
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
            return Drop(Tetriminos.GetRandom());
        }

        // returns the fallen distance of the piece
        public int Drop(Tetrimino next)
        {
            int distance = Board.DropDistance(Piece);
            // TODO: check this again!
            if (distance < 0 && Board.Intersects(Piece))
            {
                throw new GameOverException();
            }

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
        
        internal void DropUnchecked(int distance)
        {
            // let piece fall
            Piece.Fall(distance);
            Board.PlaceUnchecked(Piece);
            Pieces.Push(new Piece(Piece)); // we need to remember the dropped pieces for the landing height feature (heuristic)
            Piece = null;

            // remove lines
            int lines = Board.RemoveLines();
            Lines += lines;

            // calculate score
            Score += TetrisScore.GetSoftdropScore(distance);
            Score += TetrisScore.GetLineScore(lines, Level);
            
            if (NextPiece.HasValue)
            {
                Piece = new Piece(NextPiece.Value);
                NextPiece = Tetriminos.GetRandom();
            }
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

        public void Fall()
        {
            if (Board.Intersects(new Piece(Piece).Fall()))
                throw new GameOverException("Fall not possible");

            Piece.Fall();
        }

        public void SpawnLines(int numLines, int holePosition)
        {
            Board.SpawnLines(numLines, holePosition);

            if (Board.Intersects(Piece))
                throw new GameOverException("SpawnLines not possible");
        }

        public GameState ResetLinesAndScore()
        {
            Lines = 0;
            Score = 0;
            return this;
        }

        public override int GetHashCode()
        {
            return Lines ^ (Piece.GetHashCode() << 5) ^ ((int)NextPiece.GetValueOrDefault() << 20);
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
                builder.Append("|");

                if (y == Board.Height - 1) builder.AppendFormat(" Score: {0}", Score);
                else if (y == Board.Height - 2) builder.AppendFormat(" Level: {0}", Level);
                else if (y == Board.Height - 3) builder.AppendFormat(" Lines: {0}", Lines);
                else if (y == Board.Height - 5) builder.AppendFormat(" Curr.: {0}", Piece?.Tetrimino);
                else if (y == Board.Height - 6) builder.AppendFormat(" Next : {0}", NextPiece);

                builder.AppendLine();
            }
            builder.Append(" ");
            builder.Append(new string('-', Board.Width));
            return builder.ToString();
        }
    }
}
