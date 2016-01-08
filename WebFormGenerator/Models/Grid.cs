using System;
using System.Collections.Generic;
using Generator;
using TextParser;
using TextParser.Tokens;

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
                IToken across = child.FindFirst("Across")?.Value;
                if (across != null)
                {
                    ListToken items = across as ListToken;
                    columns = items?.Tokens[0].Convert<int>() ?? across.Convert<int>();
                    for (int i = 1; i < columns; ++i)
                        positions.MakeItemUsed(row, column + i);
                    if (items != null && items.Tokens.Count > 1)
                    {
                        rows = items.Tokens[1].Convert<int>();
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
