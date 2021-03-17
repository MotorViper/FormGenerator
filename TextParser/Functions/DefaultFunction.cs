using System;
using TextParser.Operators;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Gets the first defined value in a list.
    /// </summary>
    public class DefaultFunction : ListFunction
    {
        public DefaultFunction() : base("DEF(INED)")
        {
        }

        public override bool FinalCanBeExpression => true;

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="listToken">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        protected override IToken PerformOnList(ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            foreach (IToken token in listToken)
            {
                IToken value = token.Evaluate(substitutions, isFinal);

                if (value is ExpressionToken && !isFinal)
                    return UnParsed(listToken);

                if (!(value is NullToken) && !(value is ExpressionToken))
                    return value;
            }
            return UnParsed(listToken);
        }
    }
}
