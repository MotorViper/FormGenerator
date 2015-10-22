using System;

namespace TextParser.Tokens
{
    public class ListToken : OperatorToken
    {
        public ListToken() : base("|")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        public override TokenList Evaluate(IToken first, IToken last, TokenTreeList parameters)
        {
            TokenList firstList = first?.Evaluate(parameters);
            TokenList lastList = last?.Evaluate(parameters);

            if (firstList == null || lastList == null)
                throw new Exception($"Operation {Text} is a binary operation.");

            TokenList result = new TokenList();
            result.AddRange(firstList);
            result.AddRange(lastList);
            return result;
        }

        public override TokenList Simplify(IToken first, IToken last)
        {
            TokenList firstList = first?.Simplify();
            TokenList lastList = last?.Simplify();

            if (firstList == null || lastList == null)
                throw new Exception($"Operation {Text} is a binary operation.");

            TokenList result = new TokenList();
            result.AddRange(firstList);
            result.AddRange(lastList);
            return result;
        }
    }
}
