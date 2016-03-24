using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using System.Collections.Generic;
using System.Text;

namespace GameBot.Game.Tetris
{
    public class TetrisGameState
    {
        public Board Board { get; private set; }
        public Piece Piece { get; private set; }
        public Piece NextPiece { get; private set; }
        public Move Move { get; set; }
        
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
        }

        public TetrisGameState(TetrisGameState old, Piece piece)
        {
            Board = new Board(old.Board);
            Piece = piece;
            if (old.NextPiece != null) NextPiece = new Piece(old.NextPiece);
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
            int fall = 0;

            while (!IsPieceLanded && fall < Board.Height)
            {
                Piece.Fall();
                fall++;
            }

            Board.Place(Piece);
            Piece = null;

            if (NextPiece != null)
            {
                Piece = new Piece(NextPiece);
                NextPiece = next ?? new Piece();
            }

            return fall;
        }

        // TODO: merge with drop?
        public void RemoveLines()
        {
            Board.RemoveLines();
        }

        public IEnumerable<TetrisGameState> GetSuccessors()
        {
            var successors = new List<TetrisGameState>();

            if (Piece != null)
            {
                // TODO: use constants
                for (int translation = -4; translation < 6; translation++)
                {
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        var newPiece = new Piece(Piece.Tetromino, rotation, translation);

                        if (!Board.Intersects(newPiece))
                        {
                            var successor = new TetrisGameState(this, newPiece);
                            var fall = successor.Drop();
                            
                            successor.Move = new Move(rotation, translation, fall);                                                        
                            successors.Add(successor);
                        }
                    }
                }
            }

            return successors;
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
                    ((Board == null && other.Board == null) || Board.Equals(other.Board)) &&
                    ((Piece == null && other.Piece == null) || Piece.Equals(other.Piece)) &&
                    ((NextPiece == null && other.NextPiece == null) || NextPiece.Equals(other.NextPiece));
            }
            return false;
        }

        public override string ToString()
        {
            /*
            var sb = new StringBuilder();
            sb.AppendLine(Piece.ToString());
            sb.AppendLine(NextPiece.ToString());
            sb.Append(Board.ToString());
            return sb.ToString();*/

            var builder = new StringBuilder();
            //builder.AppendFormat("Board(p:{0})\n", Pieces);
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
