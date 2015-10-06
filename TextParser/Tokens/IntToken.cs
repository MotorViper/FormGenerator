namespace TextParser.Tokens
{
    public class IntToken : TypeToken<int>
    {
        public IntToken(int value) : base(value, TokenType.IntToken)
        {
        }
    }
}
