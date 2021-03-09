using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    public class DoubleFunction : BaseFunction
    {
        public DoubleFunction() : base("D(OUBLE)")
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
                return typeToken.ConvertToDouble(substitutions, isFinal);

            throw new Exception($"Token must be list/item of token(s) convertible to double for {Name}");
        }
    }
}
