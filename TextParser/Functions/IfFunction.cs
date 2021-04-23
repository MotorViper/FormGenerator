using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Checks the value of a token as a boolean.
    /// 2: If first true then second is returned otherwise null.
    /// 3: If first true then second is returned otherwise third.
    /// </summary>
    public class IfFunction : CheckedCountFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IfFunction() : base("IF", 2, 3)
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
        protected override IToken PerformOnList(int count, ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            IToken toCheck = listToken[0].Evaluate(substitutions, isFinal);
            if (toCheck is ExpressionToken)
                return UnParsed(listToken);

            if (!(toCheck is BoolToken query))
                throw new Exception($"First item must be boolean for '{Name}': {listToken}");

            return (query.Value ? listToken[1] : (count == 3 ? listToken[2] : new NullToken())).Evaluate(substitutions, isFinal);
        }
    }
}
