using TextParser.Tokens;

namespace TextParser.Operators
{
    public class StringPlusOperator : BaseOperator
    {
        public StringPlusOperator() : base("+")
        {
        }

        protected override IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            return new StringToken(first.Text + last.Text);
        }
    }
}
