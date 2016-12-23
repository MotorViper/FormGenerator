using TextParser.Tokens;

namespace TextParser.Operators
{
    public class StringPlusOperator : BaseOperator
    {
        public StringPlusOperator() : base("+")
        {
        }

        /// <summary>
        /// Evaluates a binary operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <returns>The evaluated value.</returns>
        protected override IToken Evaluate(ITypeToken first, ITypeToken last) => new StringToken(first.ToString() + last.ToString());
    }
}
