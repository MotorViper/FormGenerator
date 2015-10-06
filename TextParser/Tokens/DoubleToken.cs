namespace TextParser.Tokens
{
    public class DoubleToken : TypeToken<double>
    {
        public DoubleToken(double value) : base(value, TokenType.DoubleToken)
        {
        }
    }
}
