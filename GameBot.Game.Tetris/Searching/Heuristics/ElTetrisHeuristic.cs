using System;
using System.Linq;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    // Heuristic from here: http://imake.ninja/el-tetris-an-improvement-on-pierre-dellacheries-algorithm/
    public class ElTetrisHeuristic : BasicTetrisHeuristic
    {
        public override double Score(GameState gameState)
        {
            var board = gameState.Board;

            var landingHeight = LandingHeight(gameState);
            var clearedLines = gameState.Lines;
            var rowTransitions = RowTransitions(board);// - 19*2;
            var columnTransitions = ColumnTransitions(board);// - 10;
            var holes = Holes(board);
            var wellSums = WellSums(board);
            
            return
                landingHeight * -4.500158825082766 +
                clearedLines * 3.4181268101392694 +
                rowTransitions * -3.2178882868487753 +
                columnTransitions * -9.348695305445199 +
                holes * -7.899265427351652 +
                wellSums * -3.3855972247263626;
        }

        public double LandingHeight(GameState gameState)
        {
            if (gameState.Pieces.Count == 0) throw new ArgumentNullException(nameof(gameState));

            return gameState.Pieces.Average(piece =>
            {
                var landingHeight = piece.Shape.Head.Min(head => Coordinates.PieceOrigin.Y + piece.Y + head.Y);
                var pieceHeight = (piece.Shape.Height - 1.0) / 2.0;

                return landingHeight + pieceHeight;
            });
        }

        public int RowTransitions(Board board)
        {
            var transitions = 0;

            var lastColumn = Board.FullColumnMask;
            for (int x = 0; x < board.Width; x++)
            {
                var column = board.Columns[x];
                transitions += BoardLookups.Instance.GetCellCount(lastColumn ^ column);
                lastColumn = column;
            }
            transitions += BoardLookups.Instance.GetCellCount(lastColumn ^ Board.FullColumnMask);

            return transitions;
        }

        public int ColumnTransitions(Board board)
        {
            var transitions = 0;

            for (int x = 0; x < board.Width; x++)
            {
                transitions += board.ColumnTransitions(x);
            }

            return transitions;
        }

        public int WellSums(Board board)
        {
            int sum = 0;

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = board.Height - 1; y >= 0; y--)
                {
                    if (IsBoardOccupied(board, x - 1, y) && 
                        board.IsFree(x, y) && 
                        IsBoardOccupied(board, x + 1, y))
                    {
                        sum++;

                        for (int k = y - 1; k >= 0; k--)
                        {
                            if (board.IsFree(x, k))
                            {
                                sum++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            
            return sum;
        }

        private bool IsBoardOccupied(Board board, int x, int y)
        {
            if (x < 0 || x >= board.Width) return true;
            
            return board.IsOccupied(x, y);
        }
    }
}
