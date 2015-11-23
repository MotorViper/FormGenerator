using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class DoubleSumFunction : BaseFunction
    {
        public override IToken Perform(IToken dataToken, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = dataToken as ListToken;
            ITypeToken typeToken = dataToken as ITypeToken;
            double sum = 0;
            if (listToken != null)
            {
                foreach (IToken token in listToken.Tokens)
                {
                    if (token is ExpressionToken)
                        return UnParsed(listToken);
                    sum += token.Convert<int>();
                }
            }
            else if (typeToken != null)
            {
                sum = typeToken.Convert<double>();
            }
            else
            {
                throw new Exception("Token must be list or convertible to int for SUM");
            }
            return new DoubleToken(sum);
        }
    }
}
