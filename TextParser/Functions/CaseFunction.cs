using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class CaseFunction : BaseFunction
    {
        public const string ID = "CASE";

        public CaseFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = token as ListToken;
            if (listToken == null)
                throw new Exception($"Last token must be list for '{ID}'");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;

            IToken first = lastList[0];
            if (first is ExpressionToken)
                return UnParsed(listToken);

            for (int i = 1; i < count - 1; i += 2)
            {
                IToken second = lastList[i];
                if (second is ExpressionToken)
                    return UnParsed(listToken);

                if (first.Text == second.Text)
                    return lastList[i + 1];
            }

            return count % 2 == 0 ? lastList[count - 1] : new NullToken();
        }
    }
}
