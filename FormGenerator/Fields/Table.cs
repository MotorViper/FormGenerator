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
        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            properties.Add("Content");
            return properties;
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            BeginAddChildren(parameters);
            List<TokenTree> fields = GetSubFields().ToList();
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
                    Builder.AddChild(header, Level + 1, parameters, Offset, endOfLine, this);
                }
            }
            IToken over = Children.FirstOrDefault(x => x.Name == "Content")?.Value;
            if (over == null || over is NullToken)
                throw new Exception("Tried to create table with no Content.");
            if (over is StringToken)
            {
                TokenTree items = new TokenTree(DataConverter.Parameters.GetChildren(over.Text));
                foreach (TokenTree item in items.Children)
                    foreach (TokenTree child in fields)
                        Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, item.Key);
            }
            else
            {
                IToken evaluated = over.Evaluate(new TokenTreeList(parameters), true);
                ListToken list = evaluated as ListToken;
                if (list != null)
                {
                    foreach (IToken item in list.Tokens)
                        foreach (TokenTree child in fields)
                            Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, item);
                }
                else
                {
                    TokenTree items = new TokenTree(DataConverter.Parameters.GetChildren(evaluated.Text));
                    foreach (TokenTree item in items.Children)
                        foreach (TokenTree child in fields)
                            Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, item.Key);
                }
            }
            EndAddChildren();
        }
    }
}
