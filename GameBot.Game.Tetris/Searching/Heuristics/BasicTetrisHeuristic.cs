﻿using GameBot.Game.Tetris.Data;
using System;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public abstract class BasicTetrisHeuristic : IHeuristic
    {
        protected int CalculatedAggregateHeight;
        protected int CalculatedHoles;
        protected int CalculatedBumpiness;
        
        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public abstract double Score(GameState gameState);

        protected void CalculateFast(Board board)
        {
            CalculatedAggregateHeight = 0;
            CalculatedHoles = 0;
            CalculatedBumpiness = 0;

            int? lastHeight = null;

            for (int x = 0; x < board.Width; x++)
            {
                int height = board.ColumnHeightUnchecked(x);

                CalculatedAggregateHeight += height;

                CalculatedHoles += board.ColumnHolesUnchecked(x);

                if (lastHeight.HasValue)
                {
                    CalculatedBumpiness += Math.Abs(lastHeight.Value - height);
                }

                lastHeight = height;
            }
        }

        protected int Threshold(int value, int min)
        {
            return Math.Max(min, value) - min;
        }
        
        // http://www.diva-portal.se/smash/get/diva2:815662/FULLTEXT01.pdf
        protected int HolesValue(Board board, Func<int, int> f, Func<int, int> g)
        {
            int value = 0;

            for (int x = 0; x < board.Width; x++)
            {
                // check left
                if (x > 0)
                {
                    for (int y = board.ColumnHeight(x); y < board.ColumnHeight(x - 1); y++)
                    {
                        if (!board.IsOccupied(x, y))
                        {
                            value += g(y + 1);
                        }
                    }
                }

                // check right
                if (x < board.Width - 1)
                {
                    for (int y = board.ColumnHeight(x); y < board.ColumnHeight(x + 1); y++)
                    {
                        if (!board.IsOccupied(x, y))
                        {
                            value += g(y + 1);
                        }
                    }
                }

                // check current
                for (int y = 0; y < board.ColumnHeight(x); y++)
                {
                    if (!board.IsOccupied(x, y))
                    {
                        value += f(y + 1);
                    }
                }
            }

            return value;
        }

        protected int HolesValueStacking(Board board, Func<int, int> f, Func<int, int> g)
        {
            int value = 0;

            for (int x = 0; x < board.Width; x++)
            {
                // check left
                if (x > 0 && x < board.Width - 1)
                {
                    for (int y = board.ColumnHeight(x); y < board.ColumnHeight(x - 1); y++)
                    {
                        if (!board.IsOccupied(x, y))
                        {
                            value += g(y + 1);
                        }
                    }
                }

                // check right
                if (x < board.Width - 1)
                {
                    for (int y = board.ColumnHeight(x); y < board.ColumnHeight(x + 1); y++)
                    {
                        if (!board.IsOccupied(x, y))
                        {
                            value += g(y + 1);
                        }
                    }
                }

                // check current
                for (int y = 0; y < board.ColumnHeight(x); y++)
                {
                    if (!board.IsOccupied(x, y))
                    {
                        value += f(y + 1);
                    }
                }
            }

            return value;
        }

        public int MaximumHeight(Board board)
        {
            return board.MaximumHeight;
        }

        public int AggregateHeight(Board board)
        {
            int aggregateHeight = 0;
            for (int x = 0; x < board.Width; x++)
            {
                aggregateHeight += board.ColumnHeight(x);
            }
            return aggregateHeight;
        }

        public int AggregateHeightLastMultiple(Board board, int multiplicator)
        {
            int aggregateHeight = 0;
            for (int x = 0; x < board.Width - 1; x++)
            {
                aggregateHeight += board.ColumnHeight(x);
            }
            aggregateHeight += board.ColumnHeight(board.Width - 1) * multiplicator;
            return aggregateHeight;
        }

        public int CompleteLines(Board board)
        {
            return board.CompletedLines;
        }

        public int Holes(Board board)
        {
            int holes = 0;
            for (int x = 0; x < board.Width; x++)
            {
                holes += board.ColumnHolesUnchecked(x);
            }
            return holes;
        }

        public int Bumpiness(Board board)
        {
            int bumpiness = 0;
            int? lastHeight = null;
            for (int x = 0; x < board.Width; x++)
            {
                int height = board.ColumnHeight(x);
                if (lastHeight.HasValue)
                {
                    bumpiness += Math.Abs(lastHeight.Value - height);
                }
                lastHeight = height;
            }
            return bumpiness;
        }

        public int BumpinessWithoutLastColumn(Board board)
        {
            int bumpiness = 0;
            int? lastHeight = null;
            for (int x = 0; x < board.Width - 1; x++)
            {
                int height = board.ColumnHeight(x);
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
