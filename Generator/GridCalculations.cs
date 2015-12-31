using System;
using System.Collections.Generic;
using System.Linq;
using TextParser;

namespace Generator
{
    public static class GridCalculations
    {
        private static List<string> GetGridData(TokenTreeList children, string dataName, string defaultValue, string size, TokenTreeList parameters)
        {
            TokenTree fields = children.FirstOrDefault(child => child.Name == dataName);
            List<string> fieldList = null;
            if (fields != null)
            {
                fieldList = new List<string>();
                string[] fieldData = fields.Value.Evaluate(parameters, false).Text.Split(',');
                if (fieldData.Length == 1)
                {
                    int fieldCount = fields.Value.Convert<int>();
                    for (int i = 0; i < fieldCount; i++)
                        fieldList.Add(size);
                }
                else
                {
                    fieldList.AddRange(fieldData.Select(item => String.IsNullOrWhiteSpace(item) ? defaultValue : item));
                }
            }
            return fieldList;
        }

        public static List<string> GetColumnData(TokenTreeList children, TokenTreeList parameters, string columnWidth)
        {
            return GetGridData(children, "Columns", "*", columnWidth, parameters);
        }

        public static List<string> GetRowData(TokenTreeList children, TokenTreeList parameters)
        {
            return GetGridData(children, "Rows", "Auto", "Auto", parameters);
        }
    }
}
