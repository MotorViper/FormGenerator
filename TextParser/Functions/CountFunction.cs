using TextParser.Operators;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Counts the number of elements in a list. If the token is not a list there is one element.
    /// </summary>
    public class CountFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CountFunction() : base("C(OUNT)")
        {
        }

        /// <summary>
        /// Returns true if the function can work with expression tokens.
        /// </summary>
        public override bool FinalCanBeExpression => true;

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            int count = parameters == null ? 0 : 1;
            if (parameters is ListToken listToken)
                count = listToken.Count;
            return new IntToken(count);
        }

        /// <summary>
        /// The value to use if the evaluated parameter is an expression token.
        /// </summary>
        /// <param name="finalValue">The evaluated parameter.</param>
        /// <returns>The token to use.</returns>
        public override IToken ValueIfFinalValueIsExpression(ExpressionToken finalValue)
        {
            return finalValue.Operator is SubstitutionOperator ? new ListToken() : base.ValueIfFinalValueIsExpression(finalValue);
        }
    }
}
