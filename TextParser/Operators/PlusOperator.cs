using TextParser.Tokens;

namespace TextParser.Operators
{
    public class PlusOperator : BaseOperator
    {
        public PlusOperator() : base("+")
        {
        }

        public override bool CanBeUnary => true;

        protected override IToken Evaluate(ITypeToken operand)
        {
            switch (operand.Type)
            {
                case TokenType.IntToken:
                case TokenType.DoubleToken:
                    return operand;
                default:
                    return base.Evaluate(operand);
            }
        }

        /// <summary>
        /// Evaluates a binary operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <returns>The evaluated value.</returns>
        protected override IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            if (first.Type == TokenType.ListToken || last.Type == TokenType.ListToken)
            {
                ListToken firstList = first as ListToken;
                ListToken lastList = last as ListToken;
                if (firstList != null)
                {
                    if (lastList != null)
                        firstList.Value.AddRange(lastList.Value);
                    else
                        firstList.Value.Add(last);
                    return firstList;
                }

                if (lastList != null)
                {
                    lastList.Value.Insert(0, first);
                    return lastList;
                }
            }

            switch (first.Type)
            {
                case TokenType.IntToken:
                    IntToken iFirst = (IntToken)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new IntToken(iFirst.Value + ((IntToken)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(iFirst.Value + ((DoubleToken)last).Value);
                        case TokenType.StringToken:
                            return new StringToken(iFirst.Value + last.ToString());
                    }
                    break;
                case TokenType.DoubleToken:
                    DoubleToken dFirst = (DoubleToken)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new DoubleToken(dFirst.Value + ((IntToken)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(dFirst.Value + ((DoubleToken)last).Value);
                        case TokenType.StringToken:
                            return new StringToken(dFirst.Value + last.ToString());
                    }
                    break;
                case TokenType.StringToken:
                    StringToken sFirst = (StringToken)first;
                    return new StringToken(sFirst.Value + last);
            }
            return base.Evaluate(first, last);
        }
    }
}
