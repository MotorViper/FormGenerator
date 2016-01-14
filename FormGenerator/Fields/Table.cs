using System;
using System.Collections.Generic;
using System.Linq;
using FormGenerator.Tools;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class Table : Grid
    {
        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            // Content is handled differently, see below.
            properties.Add("Content");
            return properties;
        }

        /// <summary>
        /// Adds the fields children.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        protected override void AddChildren(TokenTree parameters)
        {
            BeginAddChildren(parameters);
            List<TokenTree> fields = GetSubFields().ToList();

            // Add the headers.
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
                    AddElement(header, Level + 1, parameters, this);
                }
            }

            // Add a set of children for every item in the content list.
            IToken over = Children.FirstOrDefault(x => x.Name == "Content")?.Value;
            if (over == null || over is NullToken)
                throw new Exception("Tried to create table with no Content.");
            if (over is StringToken)
            {
                TokenTree items = new TokenTree(DataConverter.Parameters.GetChildren(over.Text));
                foreach (TokenTree item in items.Children)
                    AddElements(parameters, fields, item.Key);
            }
            else
            {
                IToken evaluated = over.Evaluate(new TokenTreeList(parameters), true);
                ListToken list = evaluated as ListToken;
                if (list != null)
                {
                    foreach (IToken item in list.Tokens)
                        AddElements(parameters, fields, item);
                }
                else
                {
                    TokenTree items = new TokenTree(DataConverter.Parameters.GetChildren(evaluated.Text));
                    foreach (TokenTree item in items.Children)
                        AddElements(parameters, fields, item.Key);
                }
            }
            EndAddChildren();
        }

        /// <summary>
        /// Adds one child element per field.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="fields">The fields to add.</param>
        /// <param name="parameter">The table parameter for distinguishing each element.</param>
        private void AddElements(TokenTree parameters, List<TokenTree> fields, IToken parameter)
        {
            Parameter = parameter;
            foreach (TokenTree child in fields)
                AddElement(child, Level + 1, parameters, this);
        }
    }
}
