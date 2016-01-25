using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace Generator
{
    /// <summary>
    /// Property specified using token data.
    /// </summary>
    public class TokenTreeProperty : IProperty
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Token data.</param>
        public TokenTreeProperty(TokenTree data)
        {
            Tree = data;
        }

        /// <summary>
        /// Token data - temporary.
        /// </summary>
        public IToken Token => Tree.Value;

        /// <summary>
        /// Tree data - temporary.
        /// </summary>
        public TokenTree Tree { get; }

        /// <summary>
        /// Applies the properties that act as parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The value created by applying the parameters.</returns>
        public IValue ApplyParameters(IProperty parameters)
        {
            IToken evaluated = Token;
            if (parameters != null)
            {
                TokenTree tree = new TokenTree();
                tree.Children.Add(new TokenTree(parameters.Name, ((TokenTreeProperty)parameters).Token));
                evaluated = evaluated.SubstituteParameters(tree);
            }
            return new TokenTreeProperty(new TokenTree("", evaluated));
        }

        /// <summary>
        /// Evaluates a value using information from the element.
        /// </summary>
        /// <param name="element">The element that owns the parameter.</param>
        /// <param name="isFinal">Is this the final time the value will be evaluated.</param>
        /// <returns>The evaluated value.</returns>
        public IValue Evaluate(IElement element, bool isFinal)
        {
            IToken evaluated = Token.Evaluate((TokenTreeParameters)element.Parameters, isFinal);
            return new TokenTreeProperty(new TokenTree("", evaluated));
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name => Tree.Name;

        /// <summary>
        /// Whether the property is an integer.
        /// </summary>
        public bool IsInt => Token is IntToken;

        /// <summary>
        /// Integer value of property.
        /// </summary>
        public int IntValue => (Token as IntToken)?.Value ?? 0;

        /// <summary>
        /// String value of property.
        /// </summary>
        public string StringValue => Token.Text;

        /// <summary>
        /// Creates a property from the value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The new property.</returns>
        public IProperty CreateProperty(string name)
        {
            return new TokenTreeProperty(new TokenTree(name, Token));
        }

        /// <summary>
        /// Does the value represent an expression.
        /// </summary>
        public bool IsExpression => Token is ExpressionToken;

        /// <summary>
        /// Does the value represent a simple substitution expression.
        /// </summary>
        public bool IsVariableExpression
            => IsExpression && ((ExpressionToken)Token).Operator is SubstitutionOperator && ((ExpressionToken)Token).Second is StringToken;

        /// <summary>
        /// The variable name if IsVariableExpression is true.
        /// </summary>
        public string VariableName => ((StringToken)((ExpressionToken)Token).Second).Text;
    }
}
