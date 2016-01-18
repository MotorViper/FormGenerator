using TextParser;
using TextParser.Tokens;

namespace Generator
{
    /// <summary>
    /// Interface representing a property.
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// The property name.
        /// </summary>
        string Name { get; }

        int IntValue { get; }
        bool IsInt { get; }
        string StringValue { get; }

        IToken Token { get; }
        TokenTree Tree { get; }
    }
}
