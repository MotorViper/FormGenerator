using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Grid : Field
    {
        private List<string> _columns;
        private string _columnWidth = "*";
        private GridData _positions;
        private List<string> _rows;

        public Grid(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, "Grid", data, level, builder)
        {
        }

        protected virtual List<string> GetColumnData(TokenTreeList parameters)
        {
            return GetGridData("Columns", "*", _columnWidth, parameters);
        }

        protected List<string> GetRowData(TokenTreeList parameters)
        {
            return GetGridData("Rows", "Auto", "Auto", parameters);
        }

        protected virtual List<string> GetGridData(string dataName, string defaultValue, string size, TokenTreeList parameters)
        {
            TokenTree fields = Children.FirstOrDefault(child => child.Name == dataName);
            List<string> fieldList = null;
            if (fields != null)
            {
                fieldList = new List<string>();
                string[] fieldData = fields.Value.Evaluate(parameters).Text.Split(',');
                if (fieldData.Length == 1)
                {
                    int fieldCount = fields.Value.Convert<int>();
                    for (int i = 0; i < fieldCount; i++)
                        fieldList.Add(size);
                }
                else
                {
                    fieldList.AddRange(fieldData.Select(item => string.IsNullOrWhiteSpace(item) ? defaultValue : item));
                }
            }
            return fieldList;
        }

        protected override List<string> IgnoredProperties()
        {
            List<string> ignored = base.IgnoredProperties();
            ignored.Add("BorderThickness");
            return ignored;
        }

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
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
                        AppendStartOfLine(Level + 2, "<RowDefinition Height=\"Auto\"/>").AppendLine();
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
            IEnumerable<TokenTree> fields = BeginAddChildren(parameters);
            foreach (TokenTree child in fields)
                AddChild(child, Level + 1, parameters, Builder, Offset, endOfLine, this);
            EndAddChildren();
        }

        protected void EndAddChildren()
        {
            if (_columns != null)
                AddColumnsAndRows();
        }

        protected IEnumerable<TokenTree> BeginAddChildren(TokenTree parameters)
        {
            IEnumerable<TokenTree> fields = GetSubFields();
            TokenTreeList list = new TokenTreeList(parameters);
            _columns = GetColumnData(list);
            _rows = GetRowData(list);
            if (_rows != null && _columns == null)
                _columns = new List<string> {_columnWidth};

            if (_columns != null)
                _positions = new GridData(_columns.Count);
            return fields;
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
