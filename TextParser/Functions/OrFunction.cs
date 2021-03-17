using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    public class OrFunction : ListFunction
    {
        /// <summary>
        /// Checks if at least one element in a list is true.
        /// </summary>
        public OrFunction() : base("OR")
        {
        }

        /// <summary>
        /// Returns true if the function allows short circuit evaluation, if so no pre-evaluation is done.
        /// </summary>
        public override bool AllowsShortCircuit => true;

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        protected override IToken PerformOnList(ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            foreach (IToken t in listToken)
            {
                IToken token = t.Evaluate(substitutions, isFinal);
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                if (token.ToBool())
                    return new BoolToken(true);
            }
            return new BoolToken(false);
        }
    }
}
