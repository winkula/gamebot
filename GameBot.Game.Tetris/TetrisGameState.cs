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

        public void Drop()
        {
            // TODO: prevent infinite loop!
            while (!IsPieceLanded)
            {
                Piece.Fall();
            }
            Board.Place(Piece);
            Piece = null;

            if (NextPiece != null)
            {
                Piece = new Piece(NextPiece);
                NextPiece = null;
            }
        }
    }
}
