using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace Generator
{
    public class TokenTreeElement : IElement
    {
        private readonly TokenTree _data;
        private readonly TokenTree _parameters;
        private string _type;

        public TokenTreeElement(TokenTree data, TokenTree parameters)
        {
            _data = data;
            _parameters = parameters;
        }

        public string ElementType
        {
            set { _type = value; }
            get
            {
                if (_type == null)
                {
                    _type = _data.Value.Text;
                    TokenTree replacement = null;
                    StringToken stringToken = _data.Value as StringToken;
                    if (stringToken == null)
                    {
                        ExpressionToken expression = _data.Value as ExpressionToken;
                        if (expression != null)
                        {
                            FunctionOperator function = expression.Operator as FunctionOperator;
                            if (function != null)
                            {
                                ListToken list = (ListToken)expression.Second;
                                List<IToken> tokens = list.Tokens;
                                _type = ((ExpressionToken)tokens[0]).Second.Text;
                                TokenTree tree = new TokenTree();
                                for (int i = 1; i < tokens.Count; ++i)
                                    tree.Children.Add(new TokenTree("P" + i, tokens[i]));
                                replacement = _parameters?.FindFirst(_type);
                                replacement = replacement?.SubstituteParameters(tree);
                            }
                            else
                            {
                                _type = expression.Evaluate(new TokenTreeList(_parameters), false).Text;
                            }
                        }
                    }

                    if (replacement == null)
                        replacement = _parameters?.FindFirst(_type);

                    if (replacement != null)
                    {
                        _type = replacement.Value.Text;
                        foreach (TokenTree child in replacement.Children)
                        {
                            if (child.Name == "Field")
                                _data.Children.Add(child);
                            else
                                _data.Children.AddIfMissing(child);
                        }
                    }
                }
                return _type;
            }
        }

        public IEnumerable<IElement> Children
        {
            get { return _data.Children.Where(x => x.Name == "Field").Select(x => new TokenTreeElement(x, _parameters)); }
        }

        public IPropertyList Properties => new TokenTreePropertyList(_data, _parameters);

        public IList<IValue> GetDataList(string name)
        {
            TokenTree children = new TokenTree(_parameters.GetChildren(name));
            return children.Children.Select(child => new TokenTreeValue(child.Value)).Cast<IValue>().ToList();
        }
    }
}
