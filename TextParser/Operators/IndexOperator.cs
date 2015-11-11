using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public class IndexOperator : ListProcessingOperator
    {
        public IndexOperator() : base("#")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        protected override IToken Evaluate(IToken first, IToken last, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            IToken lastList = converter(last, parameters);
            if (lastList == null)
                throw new Exception($"Second element of Operation {Text} is not unique.");

            IntToken intToken = lastList as IntToken;
            if (intToken == null)
                throw new Exception($"Operation {Text} must have integer second element.");

            IToken tokenList = converter(first, parameters);
            ListToken listToken = tokenList as ListToken;
            return listToken == null
                ? new ExpressionToken(first, new FunctionOperator(), intToken)
                : listToken.Tokens[intToken.Value];
        }
    }
}
