using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    /// <summary>
    /// Represents a null value.
    /// </summary>
    public class NullToken : BaseToken, IConvertibleToken, ITokenWithLength
    {
        public IToken ConvertToDouble(TokenTreeList substitutions, bool isFinal)
        {
            return this;
        }

        public IToken ConvertToInt(TokenTreeList substitutions, bool isFinal)
        {
            return this;
        }

        public IntToken Count()
        {
            return new IntToken(0);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "";
        }
    }
}
