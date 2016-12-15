using System.Linq;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction
{
    public class BoardExtractor : IBoardExtractor
    {
        private const double _thresholdBlock = 0.5;
        private const double _thresholdBroken = 0.07;

        private readonly IMatcher _matcher;
        
        public BoardExtractor(IMatcher matcher)
        {
            _matcher = matcher;
        }
        
        public Board UpdateMultiplayer(IScreenshot screenshot, Board board)
        {
            int addedLines = GetAddedLines(screenshot, board);
            if (addedLines > 0)
            {
                int holePosition = FindHolePosition(screenshot, board);
                var newBoard = new Board(board);
                newBoard.InsertLinesBottom(addedLines, holePosition);

                return newBoard;
            }

            return board;
        }

        public Board Update(IScreenshot screenshot, Board board, Piece piece)
        {
            var newBoard = new Board();
            var headInBoardCoordinates = piece.Shape.Head
                .Select(h => Coordinates.PieceToBoard(piece.X + h.X, piece.Y + h.Y))
                .ToList();

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < GameBoyConstants.ScreenHeight / GameBoyConstants.TileSize; y++)
                {
                    if (headInBoardCoordinates.Any(h => h.X == x && h.Y <= y))
                    {
                        break;
                    }

                    if (_matcher.GetProbabilityBlock(screenshot, x, y) >= _thresholdBlock)
                    {
                        newBoard.Occupy(x, y);
                    }
                }
            }

            return newBoard;
        }

        public bool IsHorizonBroken(IScreenshot screenshot, Board board)
        {
            var probability = GetHorizonRaisedProbability(screenshot, board);
            return probability >= _thresholdBroken;
        }

        private int FindHolePosition(IScreenshot screenshot, Board board)
        {
            int bestPosition = 0;
            double bestProbability = double.PositiveInfinity;
            for (int x = 0; x < board.Width; x++)
            {
                var probability = _matcher.GetProbabilityBlock(screenshot, x, 0);
                if (probability < bestProbability)
                {
                    bestProbability = probability;
                    bestPosition = x;
                }
            }
            return bestPosition;
        }

        private int GetAddedLines(IScreenshot screenshot, Board board)
        {
            int numLines = 0;
            for (; numLines < 4; numLines++)
            {
                var probability = GetHorizonRaisedProbability(screenshot, board, numLines);
                if (probability < _thresholdBroken)
                {
                    break;
                }
            }
            return numLines;
        }

        private double GetHorizonRaisedProbability(IScreenshot screenshot, Board board, int height = 0)
        {
            double probabilitySum = 0.0;

            foreach (var block in board.GetHorizon(height))
            {
                if (block.Y < board.Height)
                {
                    probabilitySum += _matcher.GetProbabilityBlock(screenshot, block.X, block.Y);
                }
            }

            return probabilitySum / board.Width;
        }
    }
}
