namespace TextParser.Tokens
{
    public interface IToken
    {
        string Text { get; }
        TTo Convert<TTo>();
        IToken Simplify();
        IToken Evaluate(TokenTreeList parameters, bool isFinal);
        IToken SubstituteParameters(TokenTree parameters);

        /// <summary>
        /// Whether the token contains the input text.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>True if the current token contains the input text.</returns>
        bool Contains(string text);
    }
}
