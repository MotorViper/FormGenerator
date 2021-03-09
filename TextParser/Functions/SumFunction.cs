using TextParser.Operators;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Sums all the values in a list.
    /// </summary>
    public class SumFunction : BaseFunction
    {
        public SumFunction() : base("S(UM)")
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
            if (listToken != null)
            {
                if (listToken.Count > 0)
                {
                    IToken current = listToken[0];
                    for (int i = 1; i < listToken.Count; ++i)
                    {
                        IToken token = listToken[i];
                        current = new ExpressionToken(current, new PlusOperator(), token).Evaluate(substitutions, isFinal);
                    }
                    return current;
                }
                return new NullToken();
            }
            return parameters;
        }
    }
}
