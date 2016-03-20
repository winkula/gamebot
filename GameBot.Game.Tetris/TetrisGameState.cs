using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisGameState
    {
        public Board Board { get; private set; }
        public Piece Piece { get; private set; }
        public Piece NextPiece { get; private set; }

        public TetrisGameState()
        {
        }

        public TetrisGameState(TetrisGameState old)
        {
            if (old.Board != null) Board = new Board(old.Board);
            if (old.Piece != null) Piece = new Piece(old.Piece);
            if (old.NextPiece != null) NextPiece = new Piece(old.NextPiece);
        }

        public TetrisGameState(TetrisGameState old, Piece piece)
        {
            if (old.Board != null) Board = new Board(old.Board);
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

        public bool IsPieceLanded
        {
            get { return Board.Intersects(new Piece(Piece).Fall()); }
        }

        public int Drop()
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
                NextPiece = null;
            }

            return fall;
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
    }
}
