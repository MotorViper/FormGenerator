using Helpers;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    public class TimesOperator : BaseOperator
    {
        public TimesOperator() : base("*")
        {
        }

        protected override IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            if (first is IntToken iFirst)
            {
                if (last is IntToken iLast)
                    return new IntToken(iFirst.Value * iLast.Value);
                if (last is DoubleToken dLast)
                    return new DoubleToken(iFirst.Value * dLast.Value);
                if (last is StringToken sLast)
                    return new StringToken(StringUtils.CreateString(sLast.ToString(), iFirst.Value));
            }
            else if (first is DoubleToken dFirst)
            {
                if (last is IntToken iLast)
                    return new DoubleToken(dFirst.Value * iLast.Value);
                if (last is DoubleToken dLast)
                    return new DoubleToken(dFirst.Value * dLast.Value);
                if (last is StringToken sLast)
                    return new StringToken(StringUtils.CreateString(sLast.ToString(), dFirst.Value));
            }
            else if (first is StringToken sFirst)
            {
                if (last is IntToken iLast)
                    return new StringToken(StringUtils.CreateString(sFirst.Value, iLast.Value));
                if (last is DoubleToken dLast)
                    return new StringToken(StringUtils.CreateString(sFirst.Value, dLast.Value));
            }

            return base.Evaluate(first, last);
        }
    }
}
