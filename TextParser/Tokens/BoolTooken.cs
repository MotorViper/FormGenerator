namespace TextParser.Tokens
{
    public class BoolTooken : TypeToken<bool>
    {
        public BoolTooken(bool value) : base(value, TokenType.BoolToken)
        {
        }

        public override string Text => Value ? "True" : "False";
    }
}
