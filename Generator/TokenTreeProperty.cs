using TextParser;
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

        public TokenTree Tree { get; }

        public IValue ApplyParameters(TokenTree parameters)
        {
            IToken evaluated = Token;
            if (parameters != null)
            {
                TokenTree tree = new TokenTree();
                tree.Children.Add(parameters);
                evaluated = evaluated.SubstituteParameters(tree);
            }
            return new TokenTreeProperty(new TokenTree("", evaluated));
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name => Tree.Name;

        public bool IsInt => Token is IntToken;
        public int IntValue => (Token as IntToken)?.Value ?? 0;
        public string StringValue => Token.Text;

        public IToken Token => Tree.Value;
    }
}
