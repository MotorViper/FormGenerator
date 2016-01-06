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
        private IToken _headerBorder = new IntToken(1);
        private IToken _headerStyle;
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

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            switch (name)
            {
                case "HeaderBorder":
                    _headerBorder = value;
                    break;
                case "HeaderStyle":
                    _headerStyle = value;
                    break;
                default:
                    base.AddProperty(name, value, parameters);
                    break;
            }
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
            List<TokenTree> fields = GetSubFields().ToList();
            foreach (TokenTree child in fields)
            {
                TokenTree label = new TokenTree {Value = new StringToken("Label")};
                label.Children.Add(new TokenTree("Content", child["Header"] ?? ""));
                label.Children.Add(new TokenTree("BorderThickness", _headerBorder));
                label.Children.Add(new TokenTree("BorderBrush", "Black"));
                if (_headerStyle != null)
                    label.Children.Add(new TokenTree("Style", _headerStyle));
                Builder.AddChild(label, Level + 1, parameters, Offset, endOfLine, this);
            }
            IToken over = Children.FirstOrDefault(x => x.Name == "Content")?.Value;
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
