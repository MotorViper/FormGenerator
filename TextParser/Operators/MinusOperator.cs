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
            if (operand is IntToken iOperand)
                return new IntToken(-iOperand.Value);
            if (operand is DoubleToken dOperand)
                return new DoubleToken(-dOperand.Value);
            return base.Evaluate(operand);
        }

        /// <summary>
        /// Evaluates a binary operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <returns>The evaluated value.</returns>
        protected override IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            ListToken firstList = first as ListToken;
            ListToken lastList = last as ListToken;
            if (firstList != null || lastList != null)
            {
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

            if (first is IntToken iFirst)
            {
                if (last is IntToken iLast)
                    return new IntToken(iFirst.Value - iLast.Value);
                if (last is DoubleToken dLast)
                    return new DoubleToken(iFirst.Value - dLast.Value);
            }
            else if (first is DoubleToken dFirst)
            {
                if (last is IntToken iLast)
                    return new DoubleToken(dFirst.Value - iLast.Value);
                if (last is DoubleToken dLast)
                    return new DoubleToken(dFirst.Value - dLast.Value);
            }
            else if (first is StringToken sFirst)
            {
                return new StringToken(sFirst.Value.Replace(last.ToString(), ""));
            }
            return base.Evaluate(first, last);
        }
    }
}
