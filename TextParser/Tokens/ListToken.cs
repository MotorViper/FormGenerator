using System;

namespace TextParser.Tokens
{
    public class ListToken : ListOperatorToken
    {
        public ListToken() : base("|")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        protected override TokenList Evaluate(IToken first, IToken last, Func<IToken, TokenList> converter)
        {
            TokenList firstList = converter(first);
            TokenList lastList = converter(last);

            if (firstList == null || lastList == null)
                throw new Exception($"Operation {Text} is a binary operation.");

            TokenList result = new TokenList();
            result.AddRange(firstList);
            result.AddRange(lastList);
            return result;
        }
    }
}