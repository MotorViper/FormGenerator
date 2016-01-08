using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Tokens;

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
                var columnData = fields.Value.Evaluate(parameters, false);
                ListToken columnList = columnData as ListToken;
                if (columnList != null)
                {
                    fieldList.AddRange(columnList.Tokens.Select(item => string.IsNullOrWhiteSpace(item.Text) ? defaultValue : item.Text));
                }
                else
                {
                    IntToken columnSize = columnData as IntToken;
                    if (columnSize != null)
                    {
                        int fieldCount = columnSize.Value;
                        for (int i = 0; i < fieldCount; i++)
                            fieldList.Add(size);
                    }
                    else
                    {
                        string[] fieldData = columnData.Text.Split('|');
                        fieldList.AddRange(fieldData.Select(item => string.IsNullOrWhiteSpace(item) ? defaultValue : item));
                    }
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
