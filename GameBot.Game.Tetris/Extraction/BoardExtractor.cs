using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction
{
    public class BoardExtractor : IBoardExtractor
    {
        private readonly IMatcher _matcher;

        public BoardExtractor(IMatcher matcher)
        {
            _matcher = matcher;
        }

        public Board MultiplayerUpdate(IScreenshot screenshot, Board board)
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
            double threshold = 0.7;
            int numLines = 0;
            for (; numLines < 4; numLines++)
            {
                if (GetHorizonRaisedProbability(screenshot, board, numLines) < threshold)
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
