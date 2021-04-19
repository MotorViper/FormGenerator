using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace Generator
{
    /// <summary>
    /// A element based on token data.
    /// </summary>
    public class TokenTreeElement : IElement
    {
        private readonly TokenTree _data;
        private TokenTreeParameters _parameters;
        private string _type;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The main data.</param>
        /// <param name="parameters">Calculation data.</param>
        public TokenTreeElement(TokenTree data, TokenTreeList parameters = null)
        {
            _data = data;
            _parameters = parameters == null ? null : new TokenTreeParameters(parameters);
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
                    // Create the type from the token data.
                    IToken typeToken = _data.Value;
                    _type = typeToken.ToString();
                    TokenTree replacement = null;
                    ExpressionToken expression = _data.Value as ExpressionToken;
                    if (expression != null)
                    {
                        // The data is an expression so evaluate it.
                        FunctionOperator function = expression.Operator as FunctionOperator;
                        if (function != null)
                        {
                            // It's a function expression which in this case is treated like a template.
                            ListToken list = (ListToken)expression.Second;
                            typeToken = ((ExpressionToken)list[0]).Second;
                            _type = typeToken.ToString();
                            TokenTree tree = new TokenTree();
                            for (int i = 1; i < list.Count; ++i)
                                tree.Children.Add(new TokenTree(i.ToString(), list[i]));
                            replacement = _parameters[0]?.FindFirst(typeToken);
                            replacement = replacement?.SubstituteParameters(tree);
                        }
                        else
                        {
                            // It's an ordinary expression so evaluate it in the standard manner.
                            typeToken = expression.Evaluate(_parameters, false);
                            _type = typeToken.ToString();
                        }
                    }

                    if (replacement == null && _parameters != null)
                        replacement = _parameters[0]?.FindFirst(typeToken);

                    if (replacement != null)
                    {
                        _type = replacement.Value.ToString();
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
            get { return _data.Children.Where(x => x.Name == "Field").Select(x => new TokenTreeElement(x, _parameters)); }
        }

        /// <summary>
        /// The elements properties.
        /// </summary>
        public IPropertyList Properties => new TokenTreePropertyList(_data, _parameters[0]);

        /// <summary>
        /// The elements name.
        /// </summary>
        public string ElementName => _data.Value.ToString();

        /// <summary>
        /// Parameters used for any calculations.
        /// </summary>
        public IParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value as TokenTreeParameters; }
        }
    }
}
