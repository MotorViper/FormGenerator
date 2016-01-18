using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    /// <summary>
    /// Class to calculate the grid row and column sizes.
    /// </summary>
    public static class GridCalculations
    {
        /// <summary>
        /// Converts row/column data into a list of sizes.
        /// </summary>
        /// <param name="properties">Contains the info.</param>
        /// <param name="dataName">The name of the property containing the info.</param>
        /// <param name="defaultValue">The default size.</param>
        /// <param name="size">The actual size if a number of items is given rather than a list of sizes.</param>
        /// <returns>List of sizes.</returns>
        private static List<string> GetGridData(IPropertyList properties, string dataName, string defaultValue, string size)
        {
            IList<IProperty> property = properties.Find(dataName);
            List<string> columnList = null;
            if (property != null && property.Count > 0)
            {
                columnList = new List<string>();
                if (property.Count > 1)
                {
                    columnList.AddRange(property.Select(item => string.IsNullOrWhiteSpace(item.StringValue) ? defaultValue : item.StringValue));
                }
                else if (property[0].IsInt)
                {
                    int fieldCount = property[0].IntValue;
                    for (int i = 0; i < fieldCount; i++)
                        columnList.Add(size);
                }
                else
                {
                    string[] fieldData = property[0].StringValue.Split('|');
                    columnList.AddRange(fieldData.Select(item => string.IsNullOrWhiteSpace(item) ? defaultValue : item));
                }
            }
            return columnList;
        }

        /// <summary>
        /// Gets the grid columns sizes.
        /// </summary>
        /// <param name="properties">Holds the columns sizing information.</param>
        /// <param name="columnWidth">The default column width.</param>
        /// <returns>List of column widths.</returns>
        public static List<string> GetColumnData(IPropertyList properties, string columnWidth)
        {
            return GetGridData(properties, "Columns", "*", columnWidth);
        }

        /// <summary>
        /// Gets the grid row sizes.
        /// </summary>
        /// <param name="properties">Holds the row sizing information.</param>
        /// <returns>List of row heights.</returns>
        public static List<string> GetRowData(IPropertyList properties)
        {
            return GetGridData(properties, "Rows", "Auto", "Auto");
        }
    }
}
