using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Table : Grid
    {
        private readonly Field _border;

        public Table(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, data, level + 1, builder)
        {
            _border = new Field(parent, "Border", data, level, builder);
            _border.AddProperty("BorderThickness", 1);
            _border.AddProperty("BorderBrush", "Black");
            Parent = _border;
        }

        protected internal override void AddStart(string endOfLine, TokenTree parameters)
        {
            _border.AddStart(endOfLine, parameters);
            base.AddStart(endOfLine, parameters);
        }

        protected internal override void AddEnd(string endOfLine)
        {
            base.AddEnd(endOfLine);
            _border.AddEnd(endOfLine);
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            List<TokenTree> fields = BeginAddChildren(parameters).ToList();
            string over = Children.FirstOrDefault(x => x.Name == "Over")?.Value.Text;
            TokenTree items = new TokenTree(DataConverter.Parameters.GetChildren(over));
            foreach (TokenTree child in fields)
            {
                TokenTree label = new TokenTree {Value = new StringToken("Label")};
                label.Children.Add(new TokenTree("Content", child["Header"] ?? ""));
                label.Children.Add(new TokenTree("BorderThickness", "1"));
                label.Children.Add(new TokenTree("BorderBrush", "Black"));
                AddChild(label, Level + 1, parameters, Builder, Offset, endOfLine, this);
            }
            foreach (TokenTree item in items.Children)
                foreach (TokenTree child in fields)
                    AddChild(child, Level + 1, parameters, Builder, Offset, endOfLine, this, item.Key);
            EndAddChildren();
        }
    }
}
