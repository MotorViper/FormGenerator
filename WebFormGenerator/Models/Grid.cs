using System;
using System.Collections.Generic;
using Generator;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Class that handles output of a grid field.
    /// </summary>
    public class Grid : Field
    {
        private string _class;

        // ReSharper disable once MemberCanBeProtected.Global - ued by IOC
        /// <summary>
        /// Constructor.
        /// </summary>
        public Grid() : base("Table")
        {
        }

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        protected override void AddChildren()
        {
            GridData positions = new GridData(Element.Properties, "10");
            IEnumerable<IElement> fields = Element.Children;
            int row = 0;
            int columns = 1;
            int rows = 1;
            Builder.Append("<tr>");
            foreach (IElement child in fields)
            {
                Tuple<int, int> rowAndColumn = positions.GetNextRowAndColumn();
                if (rowAndColumn.Item1 != row)
                {
                    Builder.Append("</tr>").AppendLine();
                    Builder.Append("<tr>");
                }
                row = rowAndColumn.Item1;
                int column = rowAndColumn.Item2;
                positions.MakeItemUsed(row, column);
                IList<IProperty> across = child.Properties.Find("Across");
                if (across != null && across.Count > 0)
                {
                    columns = across[0].Value.IntValue;
                    for (int i = 1; i < columns; ++i)
                        positions.MakeItemUsed(row, column + i);
                    if (across.Count > 1)
                    {
                        rows = across[1].Value.IntValue;
                        for (int col = 0; col < columns; ++col)
                            for (int i = 0; i < rows; ++i)
                                positions.MakeItemUsed(row + i, column + col);
                    }
                }
                Builder.Append("<td ");
                if (columns > 1)
                    Builder.Append($" colspan=\"{columns}\"").AppendLine();
                if (rows > 1)
                    Builder.Append($" rowspan=\"{rows}\"").AppendLine();
                Builder.Append(">");
                AddElement(child, Level + 1, this, Keys);
                Builder.Append("</td>").AppendLine();
            }
            Builder.Append("</tr>").AppendLine();
        }

        /// <summary>
        /// Loops through the properties and outputs each one according to it's content.
        /// </summary>
        /// <param name="properties">The properties to loop over.</param>
        protected override void OutputProperties(Dictionary<string, string> properties)
        {
            base.OutputProperties(properties);
            if (_class != null)
                base.OutputProperty("class", _class + " table" + Level);
            else
                base.OutputProperty("class", "tr" + Level);
        }

        /// <summary>
        /// Outputs a single property.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <param name="value">The property value.</param>
        protected override void OutputProperty(string key, string value)
        {
            if (key == "class")
                _class = value;
            else
                base.OutputProperty(key, value);
        }
    }
}
