namespace TextParser.Tokens
{
    public class PlusToken : OperatorToken
    {
        public PlusToken() : base("+")
        {
        }

        public override bool CanBeUnary => true;

        protected override TokenList Evaluate(ITypeToken operand)
        {
            switch (operand.Type)
            {
                case TokenType.IntToken:
                    return new TokenList((IntToken)operand);
                case TokenType.DoubleToken:
                    return new TokenList((DoubleToken)operand);
            }
            return base.Evaluate(operand);
        }

        protected override TokenList Evaluate(ITypeToken first, ITypeToken last)
        {
            switch (first.Type)
            {
                case TokenType.IntToken:
                    TypeToken<int> iFirst = (TypeToken<int>)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new TokenList(new IntToken(iFirst.Value + ((TypeToken<int>)last).Value));
                        case TokenType.DoubleToken:
                            return new TokenList(new DoubleToken(iFirst.Value + ((TypeToken<double>)last).Value));
                        case TokenType.StringToken:
                            return new TokenList(new StringToken(iFirst.Value + last.ToString()));
                    }
                    break;
                case TokenType.DoubleToken:
                    TypeToken<double> dFirst = (TypeToken<double>)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new TokenList(new DoubleToken(dFirst.Value + ((TypeToken<int>)last).Value));
                        case TokenType.DoubleToken:
                            return new TokenList(new DoubleToken(dFirst.Value + ((TypeToken<double>)last).Value));
                        case TokenType.StringToken:
                            return new TokenList(new StringToken(dFirst.Value + last.ToString()));
                    }
                    break;
                case TokenType.StringToken:
                    TypeToken<string> sFirst = (TypeToken<string>)first;
                    return new TokenList(new StringToken(sFirst.Value + last));
            }
            return base.Evaluate(first, last);
        }
    }
}
