using System;
using System.Collections.Generic;

namespace Helpers
{
    public class GridData
    {
        private readonly Dictionary<int, bool> _data = new Dictionary<int, bool>();

        public GridData(int columnCount)
        {
            ColumnCount = columnCount;
        }

        public int ColumnCount { get; }
        public int RowCount { get; private set; }

        public Tuple<int, int> GetNextRowAndColumn()
        {
            int i = 0;
            bool populated;
            while (_data.TryGetValue(i, out populated) && populated)
                ++i;

            _data[i] = true;
            int column = i % ColumnCount;
            int row = (i - column) / ColumnCount;
            if (row >= RowCount)
                RowCount = row + 1;
            return Tuple.Create(row, column);
        }

        public void MakeItemUsed(int row, int column)
        {
            _data[row * ColumnCount + column] = true;
        }
    }
}
