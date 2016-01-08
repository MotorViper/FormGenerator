using System;
using System.Collections.Generic;
using Generator;
using TextParser;

namespace WebFormGenerator.Models
{
    public class Grid : Field
    {
        public Grid() : base("Table")
        {
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            GridData positions = new GridData(parameters, Children, "10");
            IEnumerable<TokenTree> fields = GetSubFields();
            int row = 0;
            int columns = 1;
            int rows = 1;
            Builder.Append("<tr>");
            foreach (TokenTree child in fields)
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
                string across = child["Across"];
                if (across != null)
                {
                    string[] bits = across.Split(',');
                    columns = int.Parse(bits[0]);
                    for (int i = 1; i < columns; ++i)
                        positions.MakeItemUsed(row, column + i);
                    if (bits.Length >= 2)
                    {
                        rows = int.Parse(bits[1]);
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
                Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, null, Selected, Keys);
                Builder.Append("</td>").AppendLine();
            }
            Builder.Append("</tr>").AppendLine();
        }
    }
}
