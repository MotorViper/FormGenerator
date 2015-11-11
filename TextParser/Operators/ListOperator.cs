using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public class ListOperator : ListProcessingOperator
    {
        public ListOperator() : base("|")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        protected override IToken Evaluate(IToken first, IToken last, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            IToken firstList = converter(first, parameters);
            IToken lastList = converter(last, parameters);

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
