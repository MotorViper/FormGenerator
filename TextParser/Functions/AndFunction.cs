using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Checks if all items in a list are true.
    /// </summary>
    public class AndFunction : BaseFunction
    {
        public const string ID = "AND";

        /// <summary>
        /// Constructor.
        /// </summary>
        public AndFunction() : base(ID)
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

            foreach (IToken t in listToken)
            {
                IToken token = t.Evaluate(substitutions, isFinal);
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                if (!token.ToBool())
                    return new BoolTooken(false);
            }
            return new BoolTooken(true);
        }
    }
}
