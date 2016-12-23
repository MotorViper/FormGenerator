using TextParser.Tokens;

namespace TextParser.Operators
{
    public interface IOperator
    {
        /// <summary>
        /// Text representation of operator.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Evaluates an operator expression.
        /// </summary>
        /// <param name="firstToken">The first value.</param>
        /// <param name="lastToken">The second value.</param>
        /// <param name="parameters">Any substitution parameters.</param>
        /// <param name="isFinal">Whether this is the final (output) call.</param>
        /// <returns>The evaluated value.</returns>
        IToken Evaluate(IToken firstToken, IToken lastToken, TokenTreeList parameters, bool isFinal);

        IToken SubstituteParameters(IToken first, IToken second, TokenTree parameters);

        /// <summary>
        /// Converts an expression token to a list of tokens if possible and required.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The list of tokens or the original token.</returns>
        IToken EvaluateList(ExpressionToken expression);
    }
}
