using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class ComparisonFunction : BaseFunction
    {
        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = token as ListToken;
            if (listToken == null)
                throw new Exception($"Last token must be list for 'COMP'");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;

            if (count < 3 || count > 5)
                throw new Exception($"Must have between 3 and 5 values for 'COMP': {listToken}");

            IToken first = lastList[0];
            IToken second = lastList[1];
            if (first is ExpressionToken || second is ExpressionToken)
                return UnParsed(listToken);

            int comparison = (first is IntToken || first is DoubleToken) && (second is IntToken || second is DoubleToken)
                ? first.Convert<double>().CompareTo(second.Convert<double>())
                : first.Text.CompareTo(second.Text);

            switch (count)
            {
                case 3:
                    return comparison == 0 ? lastList[2] : new NullToken();
                case 4:
                    return comparison == 0 ? lastList[2] : lastList[3];
                default:
                    return comparison == 1 ? lastList[2] : comparison == 0 ? lastList[3] : lastList[4];
            }
        }
    }
}
