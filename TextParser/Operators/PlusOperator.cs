using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

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
            if (operand is IntToken iOperand)
                return iOperand;
            if (operand is DoubleToken dOperand)
                return dOperand;
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

            if (first is IntToken iFirst)
            {
                if (last is IntToken iLast)
                    return new IntToken(iFirst.Value + iLast.Value);
                if (last is IntToken dLast)
                    return new DoubleToken(iFirst.Value + dLast.Value);
                if (last is StringToken sLast)
                    return new StringToken(iFirst.Value + sLast.ToString());
            }
            else if (first is DoubleToken dFirst)
            {
                if (last is IntToken iLast)
                    return new DoubleToken(dFirst.Value + iLast.Value);
                if (last is IntToken dLast)
                    return new DoubleToken(dFirst.Value + dLast.Value);
                if (last is StringToken sLast)
                    return new StringToken(dFirst.Value + sLast.ToString());
            }
            else if (first is StringToken sFirst)
            {
                return new StringToken(sFirst.Value + last);
            }
            return base.Evaluate(first, last);
        }
    }
}
