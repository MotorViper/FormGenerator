using Helpers;

namespace TextParser.Tokens
{
    public class DivideToken : OperatorToken
    {
        public DivideToken() : base("/")
        {
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
                            return new TokenList(new IntToken(iFirst.Value / ((TypeToken<int>)last).Value));
                        case TokenType.DoubleToken:
                            return new TokenList(new DoubleToken(iFirst.Value / ((TypeToken<double>)last).Value));
                    }
                    break;
                case TokenType.DoubleToken:
                    TypeToken<double> dFirst = (TypeToken<double>)first;
                    switch (last.Type)
                    {
                        case TokenType.IntToken:
                            return new TokenList(new DoubleToken(dFirst.Value / ((TypeToken<int>)last).Value));
                        case TokenType.DoubleToken:
                            return new TokenList(new DoubleToken(dFirst.Value / ((TypeToken<double>)last).Value));
                    }
                    break;
                case TokenType.StringToken:
                    TypeToken<string> sFirst = (TypeToken<string>)first;
                    switch (last.Type)
                    {
                        case TokenType.StringToken:
                            return new TokenList(new StringToken(sFirst.Value.CountInstances(((TypeToken<string>)last).Value).ToString()));
                    }
                    break;
            }
            return base.Evaluate(first, last);
        }
    }
}
