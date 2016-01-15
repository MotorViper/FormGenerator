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
        /// <summary>
        /// Constructor.
        /// </summary>
        public Grid() : base("Grid")
        {
        }

        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected override List<string> IgnoredProperties()
        {
            List<string> ignored = base.IgnoredProperties();
            ignored.Add("BorderThickness");
            return ignored;
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value"></param>
        /// <param name="parameters">Calculation parameters.</param>
        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            if (name == "ColumnWidth")
                _columnWidth = value.Text;
            else
                base.AddProperty(name, value, parameters);
        }

        /// <summary>
        /// Adds the column and row definitions for the table.
        /// </summary>
        private void AddColumnsAndRows()
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

        /// <summary>
        /// Adds the fields children.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        protected override void AddChildren(TokenTree parameters)
        {
            BeginAddChildren(parameters);
            base.AddChildren(parameters);
            EndAddChildren();
        }

        /// <summary>
        /// Outputs the row and column information if there is any.
        /// </summary>
        protected void EndAddChildren()
        {
            if (_positions.Columns != null)
                AddColumnsAndRows();
        }

        /// <summary>
        /// Start adding the grids children.
        /// </summary>
        /// <param name="parameters">The data containing the children.</param>
        protected void BeginAddChildren(TokenTree parameters)
        {
            _positions = new GridData(new TokenTreeElement(Data, parameters).Properties, _columnWidth);
        }

        /// <summary>
        /// Adds any child properties that are linked to the field.
        /// </summary>
        /// <param name="child">The child whose properties are being added.</param>
        public override void AddChildProperties(IField child)
        {
            base.AddChildProperties(child);

            // Position the child within the grid.
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
                    IToken across = tokenTreeList[0].Value;
                    if (across != null)
                    {
                        ListToken items = across as ListToken;
                        int columns = items?.Tokens[0].Convert<int>() ?? across.Convert<int>();
                        for (int i = 1; i < columns; ++i)
                            _positions.MakeItemUsed(row, column + i);
                        child.AddProperty("Grid.ColumnSpan", columns);
                        if (items != null && items.Tokens.Count > 1)
                        {
                            int rows = items.Tokens[1].Convert<int>();
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
