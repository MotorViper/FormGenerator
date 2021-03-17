using Helpers;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    public class DivideOperator : BaseOperator
    {
        public DivideOperator() : base("/")
        {
        }

        /// <summary>
        /// Evaluates a binary operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <returns>The evaluated value.</returns>
        protected override IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            if (first is IntToken iFirst)
            {
                if (last is IntToken iLast)
                    return new IntToken(iFirst.Value / iLast.Value);
                if (last is DoubleToken dLast)
                    return new DoubleToken(iFirst.Value / dLast.Value);
            }
            else if (first is DoubleToken dFirst)
            {
                if (last is IntToken iLast)
                    return new DoubleToken(dFirst.Value / iLast.Value);
                if (last is DoubleToken dLast)
                    return new DoubleToken(dFirst.Value / dLast.Value);
            }
            else if (first is StringToken sFirst)
            {
                if (!(last is ListToken))
                    return new IntToken(sFirst.Value.CountInstances(last.ToString()));
            }
            return base.Evaluate(first, last);
        }
    }
}
