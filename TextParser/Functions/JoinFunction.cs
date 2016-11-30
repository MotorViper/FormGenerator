using TextParser.Operators;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Joins a list of tokens as a string. If there is more than one token the last one is used as the separator.
    /// If any of the items in the list is also a list this is recursively expanded as well.
    /// </summary>
    public class JoinFunction : BaseFunction
    {
        public const string ID = "JOIN";

        /// <summary>
        /// Constructor.
        /// </summary>
        public JoinFunction() : base(ID)
        {
        }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            ListToken listToken = parameters as ListToken;
            int count = listToken?.Tokens.Count ?? 0;
            if (count > 0)
            {
                IToken separator = new StringToken(" ");
                if (count > 1)
                {
                    separator = listToken.Tokens[count - 1];
                    --count;
                }
                IToken current = null;
                for (int i = 0; i < count; ++i)
                {
                    IToken token = listToken.Tokens[i];
                    if (token is ExpressionToken)
                        return UnParsed(parameters);
                    current = AddToken(current, separator, token, substitutions, isFinal);
                }
                return current ?? new StringToken("");
            }
            return parameters;
        }

        private static IToken AddToken(IToken current, IToken separator, IToken toAdd, TokenTreeList parameters, bool isFinal)
        {
            ListToken currentList = toAdd as ListToken;
            if (currentList != null)
            {
                foreach (IToken tokenToAdd in currentList.Tokens)
                    current = AddToken(current, separator, tokenToAdd, parameters, isFinal);
            }
            else if (current == null)
            {
                current = toAdd;
            }
            else
            {
                current = new ExpressionToken(current, new PlusOperator(), separator).Evaluate(parameters, isFinal);
                current = new ExpressionToken(current, new PlusOperator(), toAdd).Evaluate(parameters, isFinal);
            }
            return current;
        }
    }
}
