namespace TextParser.Tokens
{
    public class StringPlusToken : OperatorToken
    {
        public StringPlusToken() : base("+")
        {
        }

        protected override TokenList Evaluate(ITypeToken first, ITypeToken last)
        {
            return new TokenList(new StringToken(first.Text + last.Text));
        }
    }
}
