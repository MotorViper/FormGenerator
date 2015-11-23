﻿using TextParser.Tokens;

namespace TextParser.Functions
{
    public class CountFunction : BaseFunction
    {
        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = token as ListToken;
            return new IntToken(listToken?.Tokens.Count ?? (token == null ? 0 : 1));
        }
    }
}