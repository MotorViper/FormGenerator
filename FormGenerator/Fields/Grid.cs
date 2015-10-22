using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Grid : Field
    {
        private List<string> _columns;
        private string _columnWidth = "Auto";
        private GridData _positions;
        private List<string> _rows;

        public Grid(Field parent, TokenTree data = null, int level = -1) : base(parent, "Grid", data, level)
        {
        }

        protected virtual List<string> GetColumnData()
        {
            return GetGridData("Columns", "*", _columnWidth);
        }

        protected List<string> GetRowData()
        {
            return GetGridData("Rows", "Auto", "Auto");
        }

        protected virtual List<string> GetGridData(string dataName, string defaultValue, string size)
        {
            TokenTree columns = Children.FirstOrDefault(child => child.Name == dataName);
            List<string> columnList = null;
            if (columns != null)
            {
                columnList = new List<string>();
                string[] columnData = columns.Value.Text.Split(',');
                if (columnData.Length == 1)
                {
                    int columnCount = columns.Value.Convert<int>();
                    for (int i = 0; i < columnCount; i++)
                        columnList.Add(size);
                }
                else
                {
                    foreach (string item in columnData)
                        columnList.Add(string.IsNullOrWhiteSpace(item) ? defaultValue : item);
                }
            }
            return columnList;
        }

        protected override List<string> IgnoredProperties()
        {
            List<string> ignored = base.IgnoredProperties();
            ignored.Add("Columns");
            ignored.Add("Rows");
            return ignored;
        }

        protected override void AddProperty(string name, IToken value, TokenTree parameters)
        {
            if (name == "ColumnWidth")
                _columnWidth = value.Text;
            else
                base.AddProperty(name, value, parameters);
        }

        protected void AddColumnsAndRows()
        {
            AppendStartOfLine(Level + 1, "<Grid.ColumnDefinitions>").AppendLine();
            foreach (string column in _columns)
                AppendStartOfLine(Level + 2, $"<ColumnDefinition Width=\"{column}\"/>").AppendLine();
            AppendStartOfLine(Level + 1, "</Grid.ColumnDefinitions>").AppendLine();

            if (_rows == null)
            {
                int rowCount = _positions.RowCount;
                if (rowCount > 1)
                {
                    AppendStartOfLine(Level + 1, "<Grid.RowDefinitions>").AppendLine();
                    for (int i = 0; i < rowCount; ++i)
                        AppendStartOfLine(Level + 2, "<RowDefinition/>").AppendLine();
                    AppendStartOfLine(Level + 1, "</Grid.RowDefinitions>").AppendLine();
                }
            }
            else
            {
                AppendStartOfLine(Level + 1, "<Grid.RowDefinitions>").AppendLine();
                foreach (string row in _rows)
                    AppendStartOfLine(Level + 2, $"<RowDefinition Height=\"{row}\"/>").AppendLine();
                AppendStartOfLine(Level + 1, "</Grid.RowDefinitions>").AppendLine();
            }
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            IEnumerable<TokenTree> fields = GetSubFields();
            _columns = GetColumnData();
            _rows = GetRowData();
            if (_rows != null && _columns == null)
                _columns = new List<string> {_columnWidth};

            if (_columns != null)
                _positions = new GridData(_columns.Count);
            foreach (TokenTree child in fields)
                AddChild(child, Level + 1, parameters, Builder, Offset, endOfLine, this);

            if (_columns != null)
                AddColumnsAndRows();
        }

        protected internal override void AddChildProperties(Field child, TokenTree parameters)
        {
            base.AddChildProperties(child, parameters);
            if (_columns != null)
            {
                Tuple<int, int> rowAndColumn = _positions.GetNextRowAndColumn();
                int column = rowAndColumn.Item2;
                int row = rowAndColumn.Item1;
                child.AddProperty("Grid.Column", column);
                child.AddProperty("Grid.Row", row);
                TokenTreeList tokenTreeList = child.Children.FindMatches("Across");
                _positions.MakeItemUsed(row, column);
                if (tokenTreeList != null && tokenTreeList.Count == 1)
                {
                    string across = tokenTreeList[0]?.Value?.Text;
                    if (across != null)
                    {
                        string[] bits = across.Split(',');
                        int columns = int.Parse(bits[0]);
                        for (int i = 1; i < columns; ++i)
                            _positions.MakeItemUsed(row, column + i);
                        child.AddProperty("Grid.ColumnSpan", columns);
                        if (bits.Length >= 2)
                        {
                            int rows = int.Parse(bits[1]);
                            child.AddProperty("Grid.RowSpan", rows);
                            for (int col = 0; col < columns; ++col)
                                for (int i = 0; i < rows; ++i)
                                    _positions.MakeItemUsed(row + i, column + col);
                        }
                    }
                }
            }
        }
    }
}
