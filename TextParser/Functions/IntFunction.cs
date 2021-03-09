using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Converts a token to an IntToken, if possible.
    /// </summary>
    public class IntFunction : BaseFunction
    {
        public IntFunction() : base("I(NT)")
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
            IConvertibleToken typeToken = parameters as IConvertibleToken;
            if (typeToken != null)
                return typeToken.ConvertToInt(substitutions, isFinal);

            throw new Exception($"Token must be list/item of tokens convertible to int for {Name}");
        }
    }
}
