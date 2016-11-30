using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Interface to be implemented by all tokens.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Returns true if the function can work with expression tokens.
        /// </summary>
        bool FinalCanBeExpression { get; }

        /// <summary>
        /// Returns true if the function does comparisons, if so no pre-evaluation is done.
        /// </summary>
        bool IsComparisonFunction { get; }

        /// <summary>
        /// The function name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal);

        /// <summary>
        /// The value to use if the evaluated parameter is an expression token.
        /// </summary>
        /// <param name="finalValue">The evaluated parameter.</param>
        /// <returns>The token to use.</returns>
        IToken ValueIfFinalValueIsExpression(ExpressionToken finalValue);
    }
}
