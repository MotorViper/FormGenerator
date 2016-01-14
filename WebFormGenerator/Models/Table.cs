using System;
using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class for displaying a table.
    /// </summary>
    public class Table : Grid
    {
        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            properties.Add("Content");
            return properties;
        }

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        /// <param name="parameters">The data used for evaluation.</param>
        protected override void AddChildren(TokenTree parameters)
        {
            List<TokenTree> fields = GetSubFields().ToList();
            Builder.Append("<tr>").AppendLine();
            foreach (TokenTree child in fields)
            {
                TokenTree header = child.FindFirst("Header");
                if (header != null)
                {
                    if (header.Children.Count == 0)
                    {
                        TokenTree label = new TokenTree {Value = new StringToken("Label")};
                        label.Children.Add(new TokenTree("Content", header.Value.Text ?? ""));
                        header = label;
                    }
                    if (header.Value is NullToken)
                        header.Value = new StringToken("Label");
                    Builder.Append("<th>").AppendLine();
                    AddElement(header, Level + 1, parameters, this);
                    Builder.Append("</th>").AppendLine();
                }
            }
            Builder.Append("</tr>").AppendLine();
            IToken over = Children.FirstOrDefault(x => x.Name == "Content")?.Value;
            if (over == null || over is NullToken)
                throw new Exception("Tried to create table with no Content.");
            if (over is StringToken)
            {
                TokenTree items = new TokenTree(parameters.GetChildren(over.Text));
                foreach (TokenTree item in items.Children)
                    AddRow(parameters, fields, item.Key);
            }
            else
            {
                IToken evaluated = over.Evaluate(new TokenTreeList(parameters), true);
                ListToken list = evaluated as ListToken;
                if (list != null)
                {
                    foreach (IToken item in list.Tokens)
                        AddRow(parameters, fields, item);
                }
                else
                {
                    TokenTree items = new TokenTree(parameters.GetChildren(evaluated.Text));
                    foreach (TokenTree item in items.Children)
                        AddRow(parameters, fields, item.Key);
                }
            }
        }

        /// <summary>
        /// Adds a single row of the table.
        /// </summary>
        /// <param name="parameters">The parameters to use for evaluating tokens.</param>
        /// <param name="fields">The fields that will be displayed.</param>
        /// <param name="item">The selected item.</param>
        private void AddRow(TokenTree parameters, List<TokenTree> fields, IToken item)
        {
            parameters = parameters.Clone();
            parameters.Children.AddIfMissing(new TokenTree("TABLEITEM", item));
            Builder.Append("<tr>").AppendLine();
            foreach (TokenTree child in fields)
            {
                Builder.Append("<td>");
                AddElement(child, Level + 1, parameters, this, Selected);
                Builder.Append("</td>");
            }
            Builder.Append("<tr>").AppendLine();
        }
    }
}
