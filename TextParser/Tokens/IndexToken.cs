using System;

namespace TextParser.Tokens
{
    public class IndexToken : OperatorToken
    {
        public IndexToken() : base("#")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        public override TokenList Evaluate(IToken first, IToken last, TokenTreeList parameters)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            TokenList lastList = last.Evaluate(parameters);
            if (lastList == null || lastList.Count != 1)
                throw new Exception($"Second element of Operation {Text} is not unique.");

            IntToken intToken = lastList[0] as IntToken;
            if (intToken == null)
                throw new Exception($"Operation {Text} must have integer second element.");

            return new TokenList(first.Evaluate(parameters)[intToken.Value]);
        }

        public override TokenList Simplify(IToken first, IToken last)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            TokenList lastList = last.Simplify();
            if (lastList == null || lastList.Count != 1)
                throw new Exception($"Second element of Operation {Text} is not unique.");

            IntToken intToken = lastList[0] as IntToken;
            if (intToken == null)
                throw new Exception($"Operation {Text} must have integer second element.");

            return new TokenList(first.Simplify()[intToken.Value]);
        }
    }
}
