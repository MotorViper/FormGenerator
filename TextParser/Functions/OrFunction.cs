using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class OrFunction : BaseFunction
    {
        public override IToken Perform(IToken parameterToken, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = parameterToken as ListToken;
            if (listToken == null)
                throw new Exception($"Last token must be list for 'OR'");

            foreach (IToken token in listToken.Tokens)
            {
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                if (token.Convert<bool>())
                    return new BoolTooken(true);
            }
            return new BoolTooken(false);
        }
    }
}
