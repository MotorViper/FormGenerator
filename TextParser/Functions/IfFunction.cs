using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Checks the value of a token as a boolean. If true the next token in the parameter list is returned otherwise the 3rd token or a null token.
    /// </summary>
    public class IfFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IfFunction() : base("IF")
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
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            ListToken listToken = parameters as ListToken;

            if (listToken == null)
                throw new Exception($"Last token must be list for '{Name}'");

            int count = listToken.Count;
            if (count != 2 && count != 3)
                throw new Exception($"Must have 2 or 3 values for '{Name}': {listToken}");

            IToken toCheck = listToken[0].Evaluate(substitutions, isFinal);
            if (toCheck is ExpressionToken)
                return UnParsed(listToken);

            BoolTooken query = toCheck as BoolTooken;
            if (query == null)
                throw new Exception($"First item must be boolean for '{Name}': {listToken}");

            return (query.Value ? listToken[1] : (count == 3 ? listToken[2] : new NullToken())).Evaluate(substitutions, isFinal);
        }
    }
}
