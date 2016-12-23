using System;

namespace GameBot.Game.Tetris.Data
{
    public class BoardLookups
    {
        private const int _size = 0x80000; // hex!

        private static BoardLookups _instance;
        public static BoardLookups Instance => _instance ?? (_instance = new BoardLookups());

        private readonly int[] _columnHeights;
        private readonly int[] _columnHoles;
        private readonly int[] _linePosition;
        private readonly int[] _columnTransitions;
        private readonly int[] _cellCount;

        private BoardLookups()
        {
            _columnHeights = new int[_size];
            _columnHoles = new int[_size];
            _linePosition = new int[_size];
            _columnTransitions = new int[_size];
            _cellCount = new int[_size];

            Init();
        }

        private void Init()
        {
            for (int i = 1; i < _size; i++)
            {
                CalculateHeights(i);
                CalculateHoles(i);
                CalculateLinePosition(i);
                CalculateColumnTransitions(i);
                CalculateCellCount(i);
            }
        }

        private void CalculateHeights(int i)
        {
            _columnHeights[i] = 1 + (int)Math.Log(i, 2);
        }

        private void CalculateHoles(int i)
        {
            int holes = 0;
            int tempCount = 0;
            for (int y = 0; y < 32; y++)
            {
                if ((i & (1 << y)) > 0)
                {
                    holes += tempCount;
                    tempCount = 0;
                }
                else
                {
                    tempCount++;
                }
            }
            _columnHoles[i] = holes;
        }

        private void CalculateLinePosition(int i)
        {
            _linePosition[i] = -1;
            for (int y = 0; y < 32; y++)
            {
                if ((i & (1 << y)) > 0)
                {
                    _linePosition[i] = y;
                    break;
                }
            }
        }

        private void CalculateColumnTransitions(int i)
        {
            int transitions = 0;
            bool lastIsBlock = true;
            for (int y = 0; y < 32; y++)
            {
                var isBlock = (i & (1 << y)) > 0;
                if (lastIsBlock != isBlock)
                {
                    transitions++;
                }

                lastIsBlock = isBlock;
            }

            _columnTransitions[i] = transitions;
        }

        private void CalculateCellCount(int i)
        {
            int count = 0;
            for (int y = 0; y < 32; y++)
            {
                if ((i & (1 << y)) > 0)
                {
                    count++;
                }
            }

            _cellCount[i] = count;
        }

        public int GetColumnHeight(int columnMask)
        {
            return _columnHeights[columnMask];
        }

        public int GetColumnHoles(int columnMask)
        {
            return _columnHoles[columnMask];
        }

        public int GetLinePosition(int columnMask)
        {
            return _linePosition[columnMask];
        }

        public int GetColumnTransitions(int columnMask)
        {
            return _columnTransitions[columnMask];
        }

        public int GetCellCount(int columnMask)
        {
            return _cellCount[columnMask];
        }
    }
}
