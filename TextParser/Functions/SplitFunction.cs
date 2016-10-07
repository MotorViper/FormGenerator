using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class SplitFunction : BaseFunction
    {
        public const string ID = "SPLIT";

        public SplitFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken parameterList, TokenTreeList parameters, bool isFinal)
        {
            string toSplit;
            int maxCount = -1;
            string[] splitOn = {" ", "\t"};
            StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
            ListToken listToken = parameterList as ListToken;

            if (listToken == null)
            {
                if (parameterList is ExpressionToken)
                    return UnParsed(parameterList);
                toSplit = parameterList.Text;
            }
            else
            {
                List<IToken> lastList = listToken.Tokens;
                int count = lastList.Count;
                if (count != 2 && count != 3)
                    throw new Exception($"Must have 1, 2 or 3 values for '{ID}': {listToken}");

                if (lastList[0] is ExpressionToken)
                    return UnParsed(listToken);

                toSplit = lastList[0].Text;
                if (count > 1)
                {
                    splitOn = new[] {lastList[1].Text};
                    options = StringSplitOptions.None;
                }
                if (count > 2)
                    maxCount = lastList[2].Convert<int>();
            }

            ListToken result = new ListToken();
            string[] bits = maxCount <= 0 ? toSplit.Split(splitOn, options) : toSplit.Split(splitOn, maxCount, options);
            foreach (string bit in bits)
                result.Add(new StringToken(bit.Trim()));
            return result;
        }
    }
}
