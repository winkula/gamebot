using System;
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
        private const double _thresholdRaisedMultiplayer = 0.6;

        private readonly IMatcher _matcher;

        public BoardExtractor(IMatcher matcher)
        {
            _matcher = matcher;
        }

        public int MultiplayerRaisedLines(IScreenshot screenshot, Board board)
        {
            return GetAddedLines(screenshot, board, _thresholdRaisedMultiplayer);
        }

        public ProbabilisticResult<int> MultiplayerHolePosition(IScreenshot screenshot, Board board)
        {
            return FindHolePositionProbabilistic(screenshot, board);
        }

        public Board MultiplayerAddLines(Board board, int raisedLines, int holePosition)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (raisedLines < 0 || raisedLines > 4) throw new ArgumentException("raisedLines must be between 0 and 4");
            if (holePosition < 0 || holePosition >= TetrisConstants.DefaultBoardWidth) throw new ArgumentException("holePosition out of range (out of board)");
            
            if (raisedLines > 0)
            {
                var newBoard = new Board(board);
                newBoard.InsertLinesBottom(raisedLines, holePosition);

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

                    if (_matcher.GetProbabilityBoardBlock(screenshot, x, y) >= _thresholdBlock)
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
                var probability = _matcher.GetProbabilityBoardBlock(screenshot, x, 0);
                if (probability < bestProbability)
                {
                    bestProbability = probability;
                    bestPosition = x;
                }
            }
            return bestPosition;
        }

        private ProbabilisticResult<int> FindHolePositionProbabilistic(IScreenshot screenshot, Board board)
        {
            int bestPosition = 0;
            double bestProbability = double.PositiveInfinity;
            for (int x = 0; x < board.Width; x++)
            {
                var probability = _matcher.GetProbabilityBoardBlock(screenshot, x, 0);
                if (probability < bestProbability)
                {
                    bestProbability = probability;
                    bestPosition = x;
                }
            }
            return new ProbabilisticResult<int>(bestPosition, bestProbability);
        }

        private int GetAddedLines(IScreenshot screenshot, Board board, double threshold)
        {
            int numLines = 0;
            for (; numLines < 4; numLines++)
            {
                var probability = GetHorizonRaisedProbability(screenshot, board, numLines);
                if (probability < threshold)
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
                    probabilitySum += _matcher.GetProbabilityBoardBlock(screenshot, block.X, block.Y);
                }
            }

            return probabilitySum / board.Width;
        }
    }
}
