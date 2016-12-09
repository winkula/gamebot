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

        private BoardLookups()
        {
            _columnHeights = new int[_size];
            _columnHoles = new int[_size];
            _linePosition = new int[_size];

            Init();
        }

        private void Init()
        {
            for (int i = 1; i < _size; i++)
            {
                CalculateHeights(i);
                CalculateHoles(i);
                CalculateLinePosition(i);
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

        public int GetColumnHeight(int column)
        {
            return _columnHeights[column];
        }

        public int GetColumnHoles(int column)
        {
            return _columnHoles[column];
        }

        public int GetLinePosition(int columns)
        {
            return _linePosition[columns];
        }
    }
}
