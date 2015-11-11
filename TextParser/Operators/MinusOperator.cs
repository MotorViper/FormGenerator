using TextParser.Tokens;

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
                            return new IntToken(iFirst.Value - ((TypeToken<int>)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(iFirst.Value - ((TypeToken<double>)last).Value);
                    }
                    break;
                case TokenType.DoubleToken:
                    TypeToken<double> dFirst = (TypeToken<double>)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new DoubleToken(dFirst.Value - ((TypeToken<int>)last).Value);
                        case TokenType.DoubleToken:
                            return new DoubleToken(dFirst.Value - ((TypeToken<double>)last).Value);
                    }
                    break;
                case TokenType.StringToken:
                    TypeToken<string> sFirst = (TypeToken<string>)first;
                    switch (last.Type)
                    {
                        case TokenType.StringToken:
                            return new StringToken(sFirst.Value.Replace(((TypeToken<string>)last).Value, ""));
                    }
                    break;
            }
            return base.Evaluate(first, last);
        }
    }
}
