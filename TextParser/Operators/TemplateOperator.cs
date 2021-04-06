using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    /// <summary>
    /// Operator for processing a template.
    /// </summary>
    public class TemplateOperator : BaseOperator
    {
        public TemplateOperator() : base("::")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        /// <summary>
        /// Evaluates an operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <param name="parameters">Any substitution parameters.</param>
        /// <param name="isFinal">Whether this is the final (output) call.</param>
        /// <returns>The evaluated value.</returns>
        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal)
        {
            return new StringToken(first.ToString() + "::" + last.ToString());
        }
    }
}
