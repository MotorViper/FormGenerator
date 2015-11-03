using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextParser.Tokens
{
    public class FunctionToken : ListOperatorToken
    {
        public FunctionToken() : base(":")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        protected override TokenList Evaluate(IToken first, IToken last, Func<IToken, TokenList> converter)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            TokenList firstList = converter(first);
            if (firstList == null || firstList.Count != 1)
                throw new Exception($"First element of Operation {Text} is not unique.");

            string function = firstList[0].Text;
            TokenList lastList = converter(last);

            if (lastList.Count == 1 && lastList[0] is ExpressionToken)
                return new TokenList(new ExpressionToken(new StringToken(function), new FunctionToken(), lastList[0]));
            else if (lastList.Any(x => x is ExpressionToken))
                return new TokenList(new ExpressionToken(new StringToken(function), new FunctionToken(), last));

            switch (function.ToUpper())
            {
                case "OVER":
                    return new TokenList(new StringToken("IDunno"));
                case "COUNT":
                    return new TokenList(new IntToken(lastList.Count));
                case "AGG":
                    return new TokenList(new StringToken(lastList.Aggregate()));
                case "SUMI":
                case "SUM":
                    return new TokenList(new IntToken(lastList.Sum()));
                case "SUMD":
                    return new TokenList(new DoubleToken(lastList.DSum()));
                default:
                    throw new Exception($"{function} is not recognised for {Text}");
            }
        }
    }
}
