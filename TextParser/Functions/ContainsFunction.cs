using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class ContainsFunction : BaseFunction
    {
        public override IToken Perform(IToken parameterList, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = parameterList as ListToken;
            ;
            if (listToken == null)
                throw new Exception("Last token must be list for CONTAINS");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            IToken toFind = lastList[count - 1];
            for (int i = 0; i < count - 1; i++)
            {
                IToken token = lastList[i];
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                ListToken list = token as ListToken;
                if (list != null && list.Tokens.Contains(toFind))
                    return new BoolTooken(true);
                if (token.Text == toFind.Text)
                    return new BoolTooken(true);
            }
            return new BoolTooken(false);
        }
    }
}
