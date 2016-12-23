﻿using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Reverses the items in a list.
    /// </summary>
    public class ReverseFunction : BaseFunction
    {
        public ReverseFunction() : base("REV(ERSE)")
        {
        }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            ListToken list = parameters as ListToken ?? new ListToken(parameters);
            ListToken result = new ListToken();
            result.Value.AddRange(list.Value);
            result.Value.Reverse();
            return result;
        }
    }
}
