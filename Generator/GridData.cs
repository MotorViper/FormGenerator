using System;
using System.Collections.Generic;

namespace Generator
{
    /// <summary>
    /// This class is used to calculate where in a grid the next available space is.
    /// </summary>
    public class GridData
    {
        private readonly Dictionary<int, bool> _data = new Dictionary<int, bool>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="children">The children that need to be included in the grid.</param>
        /// <param name="columnWidth">The width of each column.</param>
        public GridData(IPropertyList children, string columnWidth)
        {
            List<string> columns = GridCalculations.GetColumnData(children, columnWidth);
            List<string> rows = GridCalculations.GetRowData(children);

            if (rows != null && columns == null)
                columns = new List<string> {columnWidth};
            Rows = rows;
            Columns = columns;
            ColumnCount = Columns?.Count ?? 1;
        }

        /// <summary>
        /// How many columns the grid will span.
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// Information on the sizing of each column.
        /// </summary>
        public List<string> Columns { get; }

        /// <summary>
        /// How many rows the grid will span.
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Information on the sizing of each row.
        /// </summary>
        public List<string> Rows { get; }

        /// <summary>
        /// Gets the next free row and column.
        /// </summary>
        /// <returns>A tuple containing the row and column.</returns>
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

        /// <summary>
        /// Indicate that the input row and column are not available.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="column">The column number.</param>
        public void MakeItemUsed(int row, int column)
        {
            _data[row * ColumnCount + column] = true;
        }
    }
}
