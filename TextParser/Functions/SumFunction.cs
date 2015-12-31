using TextParser.Operators;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class SumFunction : BaseFunction
    {
        public const string ID = "SUM";

        public SumFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken dataToken, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = dataToken as ListToken;
            if (listToken != null)
            {
                IToken current = listToken.Tokens[0];
                for (int i = 1; i < listToken.Tokens.Count; ++i)
                {
                    IToken token = listToken.Tokens[i];
                    current = new ExpressionToken(current, new PlusOperator(), token).Evaluate(parameters, isFinal);
                }
                return current;
            }
            return dataToken;
        }
    }
}
