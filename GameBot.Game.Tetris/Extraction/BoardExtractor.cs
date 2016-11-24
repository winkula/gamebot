using System;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction
{
    public class BoardExtractor : IBoardExtractor
    {
        private const double _threshold = 0.7;

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
            throw new NotImplementedException();
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
                if (GetHorizonRaisedProbability(screenshot, board, numLines) < _threshold)
                {
                    break;
                }
            }
            return numLines;
        }

        private double GetHorizonRaisedProbability(IScreenshot screenshot, Board board, int height)
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
