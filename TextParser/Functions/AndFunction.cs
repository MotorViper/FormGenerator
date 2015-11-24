using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class AndFunction : BaseFunction
    {
        public const string ID = "AND";

        public AndFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken parameterToken, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = parameterToken as ListToken;
            if (listToken == null)
                throw new Exception($"Last token must be list for '{ID}'");

            foreach (IToken token in listToken.Tokens)
            {
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                if (!token.Convert<bool>())
                    return new BoolTooken(false);
            }
            return new BoolTooken(true);
        }
    }
}
