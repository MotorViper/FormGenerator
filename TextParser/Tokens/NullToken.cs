using System;

namespace TextParser.Tokens
{
    public class NullToken : BaseToken
    {
        public override string Text => "";

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

        public override TTo Convert<TTo>()
        {
            throw new Exception("Cannot convert null token");
        }
    }
}
