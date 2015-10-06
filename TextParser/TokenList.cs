using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser
{
    public class TokenList : List<IToken>
    {
        public TokenList(IToken token)
        {
            Add(token);
        }

        public TokenList()
        {
        }
    }
}
