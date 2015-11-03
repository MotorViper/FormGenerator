using System;

namespace TextParser.Tokens
{
    public class IndexToken : ListOperatorToken
    {
        public IndexToken() : base("#")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        protected override TokenList Evaluate(IToken first, IToken last, Func<IToken, TokenList> converter)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            TokenList lastList = converter(last);
            if (lastList == null || lastList.Count != 1)
                throw new Exception($"Second element of Operation {Text} is not unique.");

            IntToken intToken = lastList[0] as IntToken;
            if (intToken != null)
                throw new Exception($"Operation {Text} must have integer second element.");

            return new TokenList(converter(first)[intToken.Value]);
        }
    }
}
