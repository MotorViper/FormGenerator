using System;
using System.Collections.Generic;
using TextParser;

namespace Generator
{
    public class GridData
    {
        private readonly Dictionary<int, bool> _data = new Dictionary<int, bool>();

        public GridData(TokenTree parameters, TokenTreeList children, string columnWidth)
        {
            TokenTreeList list = new TokenTreeList(parameters);
            List<string> columns = GridCalculations.GetColumnData(children, list, columnWidth);
            List<string> rows = GridCalculations.GetRowData(children, list);

            if (rows != null && columns == null)
                columns = new List<string> {columnWidth};
            Rows = rows;
            Columns = columns;
            ColumnCount = Columns?.Count ?? 0;
        }

        public int ColumnCount { get; }
        public List<string> Columns { get; }
        public int RowCount { get; private set; }
        public List<string> Rows { get; }

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
