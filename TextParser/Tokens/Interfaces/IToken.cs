using TextParser.Functions;

namespace TextParser.Tokens.Interfaces
{
    /// <summary>
    /// Interface for all tokens to implement.
    /// </summary>
    public interface IToken //: IComparable<IToken>
    {
        //ITreeToken Parent { get; set; }

        /// <summary>
        /// Whether this token contains an expression.
        /// </summary>
        bool IsExpression { get; }

        /// <summary>
        /// Whether the contained value should be treated as a single element.
        /// </summary>
        bool Verbatim { get; }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        double ToDouble();

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        int ToInt();

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        bool ToBool();

        /// <summary>
        /// Converts the token to a string.
        /// </summary>
        string ToString();

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
        IToken Flatten();

        IToken SubstituteParameters(TokenTree parameters);

        void ModifyParameters(UserFunction function);

        /// <summary>
        /// Whether the token contains the input token.
        /// </summary>
        /// <param name="token">The input token.</param>
        /// <returns>True if the current token contains the input token.</returns>
        bool HasMatch(IToken token);
    }
}
