using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class IfFunction : BaseFunction
    {
        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = token as ListToken;

            if (listToken == null)
                throw new Exception($"Last token must be list for 'IF'");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            if (count != 2 && count != 3)
                throw new Exception($"Must have 2 or 3 values for 'IF': {listToken}");

            if (lastList[0] is ExpressionToken)
                return UnParsed(listToken);

            BoolTooken query = lastList[0] as BoolTooken;
            if (query == null)
                throw new Exception($"First item must be boolean for 'IF': {listToken}");

            return query.Value ? lastList[1] : (count == 3 ? lastList[2] : new NullToken());
        }
    }
}
