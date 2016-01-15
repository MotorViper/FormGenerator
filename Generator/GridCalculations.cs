using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    public static class GridCalculations
    {
        private static List<string> GetGridData(IPropertyList properties, string dataName, string defaultValue, string size)
        {
            IProperty property = properties.Find(dataName);
            List<string> columnList = null;
            if (property != null)
            {
                columnList = new List<string>();
                if (property.IsList)
                {
                    columnList.AddRange(property.Values.Select(item => string.IsNullOrWhiteSpace(item.StringValue) ? defaultValue : item.StringValue));
                }
                else if (property.Value.IsInt)
                {
                    int fieldCount = property.Value.IntValue;
                    for (int i = 0; i < fieldCount; i++)
                        columnList.Add(size);
                }
                else
                {
                    string[] fieldData = property.Value.StringValue.Split('|');
                    columnList.AddRange(fieldData.Select(item => string.IsNullOrWhiteSpace(item) ? defaultValue : item));
                }
            }
            return columnList;
        }

        public static List<string> GetColumnData(IPropertyList children, string columnWidth)
        {
            return GetGridData(children, "Columns", "*", columnWidth);
        }

        public static List<string> GetRowData(IPropertyList children)
        {
            return GetGridData(children, "Rows", "Auto", "Auto");
        }
    }
}
