﻿using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;

namespace GameBot.Game.Tetris
{
    public class TetrisHeuristic : IHeuristic<TetrisGameState>
    {
        public double Score(TetrisGameState gameState)
        {
            var board = gameState.Board;
            return 1.0 / AggregateHeight(board);
        }

        // top down
        public int Height(Board board, int x)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (x < 0 || x >= board.Width) throw new ArgumentException(nameof(x));

            for (int y = board.Height - 1; y >= 0; y--)
            {
                if (board.IsOccupied(x, y))
                {
                    return (y + 1);
                }
            }

            return 0;
        }

        // top down
        public int AggregateHeight(Board board)
        {
            int aggregateHeight = 0;
            for (int x = 0; x < board.Width; x++)
            {
                aggregateHeight += Height(board, x);
            }
            return aggregateHeight;
        }

        // bottom up
        public int CompleteLines(Board board)
        {
            int completeLines = 0;
            for (int y = 0; y < board.Height; y++)
            {
                bool lineFound = true;
                for (int x = 0; x < board.Width; x++)
                {
                    if (!board.IsOccupied(x, y))
                    {
                        lineFound = false;
                        break;
                    }
                }
                if (lineFound) completeLines++;
            }
            return completeLines;
        }

        // bottom up
        public int Holes(Board board)
        {
            int holes = 0;
            for (int x = 0; x < board.Width; x++)
            {
                int tempCount = 0;
                for (int y = 0; y < board.Height; y++)
                {
                    if (board.IsOccupied(x, y))
                    {
                        holes += tempCount;
                        tempCount = 0;
                    }
                    else
                    {
                        tempCount++;
                    }
                }
            }
            return holes;
        }

        // top down
        public int Bumpiness(Board board)
        {
            int bumpiness = 0;
            int? lastHeight = null;
            for (int x = 0; x < board.Width; x++)
            {
                int height = Height(board, x);
                if (lastHeight.HasValue)
                {
                    bumpiness += Math.Abs(lastHeight.Value - height);
                }
                lastHeight = height;
            }
            return bumpiness;
        }
    }
}
