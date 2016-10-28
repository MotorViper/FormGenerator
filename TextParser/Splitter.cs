using System;
using Helpers;

namespace TextParser
{
    /// <summary>
    /// Splits the input line into a key and a value token.
    /// </summary>
    public static class Splitter
    {
        public static TokenTree Split(string text, bool ignoreErrors = false)
        {
            int pos = text.FirstNotInBlock(':', new[] {'\'', '"', '^'});
            if (pos < 0)
            {
                if (ignoreErrors)
                    return new TokenTree(text.Trim(), "");
                throw new Exception("Invalid format: Missing ':'");
            }
            return new TokenTree(text.Substring(0, pos).Trim(), text.Substring(pos + 1).Trim());
        }
    }
}
