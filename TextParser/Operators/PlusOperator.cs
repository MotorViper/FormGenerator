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
                    return (IntToken)operand;
                case TokenType.DoubleToken:
                    return (DoubleToken)operand;
            }
            return base.Evaluate(operand);
        }

        protected override IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            switch (first.Type)
            {
                case TokenType.IntToken:
                    TypeToken<int> iFirst = (TypeToken<int>)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new IntToken(iFirst.Value + ((TypeToken<int>)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(iFirst.Value + ((TypeToken<double>)last).Value);
                        case TokenType.StringToken:
                            return new StringToken(iFirst.Value + last.ToString());
                    }
                    break;
                case TokenType.DoubleToken:
                    TypeToken<double> dFirst = (TypeToken<double>)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new DoubleToken(dFirst.Value + ((TypeToken<int>)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(dFirst.Value + ((TypeToken<double>)last).Value);
                        case TokenType.StringToken:
                            return new StringToken(dFirst.Value + last.ToString());
                    }
                    break;
                case TokenType.StringToken:
                    TypeToken<string> sFirst = (TypeToken<string>)first;
                    return new StringToken(sFirst.Value + last);
            }
            return base.Evaluate(first, last);
        }
    }
}
