using System;
using System.Collections.Generic;
using Generator;

namespace FormGenerator.Fields
{
    /// <summary>
    /// Class representing a grid.
    /// </summary>
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
        protected override void AddProperty(string name, IValue value)
        {
            if (name == "ColumnWidth")
                _columnWidth = value.StringValue;
            else
                base.AddProperty(name, value);
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
        protected override void AddChildren()
        {
            BeginAddChildren();
            base.AddChildren();
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
        protected void BeginAddChildren()
        {
            _positions = new GridData(Element.Properties, _columnWidth);
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
                child.AddTypedProperty("Grid.Column", column);
                child.AddTypedProperty("Grid.Row", row);
                IList<IProperty> properties = child.Element.Properties.Find("Across");
                _positions.MakeItemUsed(row, column);
                if (properties != null && properties.Count > 0)
                {
                    IProperty across = properties[0];
                    if (across != null)
                    {
                        int columns = across.IntValue;
                        for (int i = 1; i < columns; ++i)
                            _positions.MakeItemUsed(row, column + i);
                        child.AddTypedProperty("Grid.ColumnSpan", columns);
                        if (properties.Count > 1)
                        {
                            int rows = properties[1].IntValue;
                            child.AddTypedProperty("Grid.RowSpan", rows);
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
