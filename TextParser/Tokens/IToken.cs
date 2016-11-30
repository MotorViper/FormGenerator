namespace TextParser.Tokens
{
    /// <summary>
    /// Interface for all tokens to implement.
    /// </summary>
    public interface IToken
    {
        string Text { get; }
        TTo Convert<TTo>();

        /// <summary>
        /// Simplifies the expression as much as possible.
        /// </summary>
        /// <returns></returns>
        IToken Simplify();

        /// <summary>
        /// Evaluates the token.
        /// </summary>
        /// <param name="parameters">The parameters to use for substitutions.</param>
        /// <param name="isFinal">Whether this is a final parse.</param>
        /// <returns></returns>
        IToken Evaluate(TokenTreeList parameters, bool isFinal);

        /// <summary>
        /// Converts the token to a list of tokens if possible and required.
        /// </summary>
        /// <returns>The list of tokens or the original token.</returns>
        IToken EvaluateList();

        IToken SubstituteParameters(TokenTree parameters);

        /// <summary>
        /// Whether the token contains the input text.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>True if the current token contains the input text.</returns>
        bool Contains(string text);
    }
}
