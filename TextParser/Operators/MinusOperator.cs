using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    public class MinusOperator : BaseOperator
    {
        public MinusOperator() : base("-")
        {
        }

        public override bool CanBeUnary => true;

        protected override IToken Evaluate(ITypeToken operand)
        {
            switch (operand.Type)
            {
                case TokenType.IntToken:
                    return new IntToken(-((IntToken)operand).Value);
                case TokenType.DoubleToken:
                    return new DoubleToken(-((DoubleToken)operand).Value);
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
                    if (lastList == null)
                    {
                        firstList.Value.RemoveAll(token => token.ToString() == last.ToString());
                    }
                    else
                    {
                        foreach (IToken item in lastList.Value)
                            firstList.Value.RemoveAll(token => token.ToString() == item.ToString());
                    }
                    return firstList;
                }

                if (lastList != null)
                {
                    string value = first.ToString();
                    foreach (IToken token in lastList)
                        value = value.Replace(token.ToString(), "");
                    return new StringToken(value);
                }
            }


            switch (first.Type)
            {
                case TokenType.IntToken:
                    IntToken iFirst = (IntToken)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new IntToken(iFirst.Value - ((IntToken)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(iFirst.Value - ((DoubleToken)last).Value);
                    }
                    break;
                case TokenType.DoubleToken:
                    DoubleToken dFirst = (DoubleToken)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new DoubleToken(dFirst.Value - ((IntToken)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(dFirst.Value - ((DoubleToken)last).Value);
                    }
                    break;
                case TokenType.StringToken:
                    StringToken sFirst = (StringToken)first;
                    return new StringToken(sFirst.Value.Replace(last.ToString(), ""));
            }
            return base.Evaluate(first, last);
        }
    }
}
