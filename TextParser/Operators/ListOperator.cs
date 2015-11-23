using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public class ListOperator : BaseOperator
    {
        public ListOperator() : base("|")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal)
        {
            IToken firstList = first.Evaluate(parameters, isFinal);
            IToken lastList = last.Evaluate(parameters, isFinal);

            if (firstList == null || lastList == null)
                throw new Exception($"Operation {Text} is a binary operation.");

            ListToken result = new ListToken();
            ListToken tokens = firstList as ListToken;
            if (tokens != null)
                result.Tokens.AddRange(tokens.Tokens);
            else
                result.Tokens.Add(firstList);
            tokens = lastList as ListToken;
            if (tokens != null)
                result.Tokens.AddRange(tokens.Tokens);
            else
                result.Add(lastList);
            return result;
        }
    }
}
