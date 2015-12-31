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
        private Field _border;
        private TokenTree _data;
        private int _level;

        public override TokenTree Data
        {
            set
            {
                _data = value;
                base.Data = value;
            }
        }

        public override int Level
        {
            protected get { return _level; }
            set { _level = value + 1; }
        }

        public override void AddStart(string endOfLine, TokenTree parameters)
        {
            _border = new Border
            {
                Level = Level - 1,
                Parent = Parent,
                Data = _data
            };
            _border.AddProperty("BorderThickness", 1);
            _border.AddProperty("BorderBrush", "Black");
            Parent = _border;
            _border.Builder = Builder;
            _border.AddStart(endOfLine, parameters);
            base.AddStart(endOfLine, parameters);
        }

        public override void AddEnd(string endOfLine)
        {
            base.AddEnd(endOfLine);
            _border.AddEnd(endOfLine);
        }

        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            properties.Add("Content");
            return properties;
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            BeginAddChildren(parameters);
            string over = Children.FirstOrDefault(x => x.Name == "Content")?.Value.Text;
            TokenTree items = new TokenTree(DataConverter.Parameters.GetChildren(over));
            List<TokenTree> fields = GetSubFields().ToList();
            foreach (TokenTree child in fields)
            {
                TokenTree label = new TokenTree {Value = new StringToken("Label")};
                label.Children.Add(new TokenTree("Content", child["Header"] ?? ""));
                label.Children.Add(new TokenTree("BorderThickness", "1"));
                label.Children.Add(new TokenTree("BorderBrush", "Black"));
                Builder.AddChild(label, Level + 1, parameters, Offset, endOfLine, this);
            }
            foreach (TokenTree item in items.Children)
                foreach (TokenTree child in fields)
                    Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, item.Key);
            EndAddChildren();
        }
    }
}
