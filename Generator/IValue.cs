using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public interface IValue
    {
        /// <summary>
        /// Integer value of property.
        /// </summary>
        int IntValue { get; }

        /// <summary>
        /// Whether the property is an integer.
        /// </summary>
        bool IsInt { get; }

        /// <summary>
        /// String value of property.
        /// </summary>
        string StringValue { get; }

        #region Temporary

        IToken Token { get; }
        TokenTree Tree { get; }

        IValue ApplyParameters(TokenTree parameters);

        #endregion
    }
}
