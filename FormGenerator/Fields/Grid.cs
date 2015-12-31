using System;
using System.Collections.Generic;
using Generator;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Grid : Field
    {
        private string _columnWidth = "*";
        private GridData _positions;

        // ReSharper disable once MemberCanBeProtected.Global - used by IOC.
        public Grid() : base("Grid")
        {
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
            foreach (string column in _positions.Columns)
                AppendStartOfLine(Level + 2, $"<ColumnDefinition Width=\"{column}\"/>").AppendLine();
            AppendStartOfLine(Level + 1, "</Grid.ColumnDefinitions>").AppendLine();

            if (_positions.Rows == null)
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
                foreach (string row in _positions.Rows)
                    AppendStartOfLine(Level + 2, $"<RowDefinition Height=\"{row}\"/>").AppendLine();
                AppendStartOfLine(Level + 1, "</Grid.RowDefinitions>").AppendLine();
            }
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            BeginAddChildren(parameters);
            base.AddChildren(parameters, endOfLine);
            EndAddChildren();
        }

        protected void EndAddChildren()
        {
            if (_positions.Columns != null)
                AddColumnsAndRows();
        }

        protected void BeginAddChildren(TokenTree parameters)
        {
            _positions = new GridData(parameters, Children, _columnWidth);
        }

        public override void AddChildProperties(IField child)
        {
            base.AddChildProperties(child);
            if (_positions.Columns != null)
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
