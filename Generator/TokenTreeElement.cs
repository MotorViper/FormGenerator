using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace Generator
{
    /// <summary>
    /// A element based on token data.
    /// </summary>
    public class TokenTreeElement : IElement
    {
        private readonly TokenTree _data;
        private string _type;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The main data.</param>
        /// <param name="parameters">Calculation data.</param>
        public TokenTreeElement(TokenTree data, TokenTreeList parameters)
        {
            _data = data;
            Parameters = parameters;
        }

        /// <summary>
        /// The elements type.
        /// </summary>
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
                                replacement = Parameters[0]?.FindFirst(_type);
                                replacement = replacement?.SubstituteParameters(tree);
                            }
                            else
                            {
                                _type = expression.Evaluate(Parameters, false).Text;
                            }
                        }
                    }

                    if (replacement == null)
                        replacement = Parameters[0]?.FindFirst(_type);

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

        /// <summary>
        /// The elements children.
        /// </summary>
        public IEnumerable<IElement> Children
        {
            get { return _data.Children.Where(x => x.Name == "Field").Select(x => new TokenTreeElement(x, Parameters)); }
        }

        /// <summary>
        /// The elements properties.
        /// </summary>
        public IPropertyList Properties => new TokenTreePropertyList(_data, Parameters[0]);

        /// <summary>
        /// The elements name.
        /// </summary>
        public string ElementName => _data.Value.Text;

        public TokenTreeList Parameters { get; set; }
    }
}
