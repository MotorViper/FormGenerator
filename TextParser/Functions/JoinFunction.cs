using TextParser.Operators;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Joins a list of tokens as a string. If there is more than one token the last one is used as the separator.
    /// </summary>
    public class JoinFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JoinFunction() : base("J(OIN)")
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
            if (parameters is ListToken listToken)
            {
                int count = listToken.Count;
                IToken separator = new StringToken(" ");
                if (count > 1)
                {
                    separator = listToken[count - 1];
                    --count;
                }
                IToken current = null;
                for (int i = 0; i < count; ++i)
                {
                    IToken token = listToken[i];
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
                foreach (IToken tokenToAdd in currentList)
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
