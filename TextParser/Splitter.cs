using Helpers;

namespace TextParser
{
    public class Splitter
    {
        public TokenTree Split(string text)
        {
            int pos = text.FirstNotInBlock(':');
            return new TokenTree(text.Substring(0, pos).Trim(), text.Substring(pos + 1).Trim());
        }
    }
}
