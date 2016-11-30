using TextParser.Tokens;

namespace TextParser.Operators
{
    public interface IOperator
    {
        /// <summary>
        /// Text representation of operator.
        /// </summary>
        string Text { get; }

        IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal);
        IToken SubstituteParameters(IToken first, IToken second, TokenTree parameters);

        /// <summary>
        /// Converts an expression token to a list of tokens if possible and required.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The list of tokens or the original token.</returns>
        IToken EvaluateList(ExpressionToken expression);
    }
}
