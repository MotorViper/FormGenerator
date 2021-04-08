using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    public abstract class ListFunction : BaseFunction
    {
        protected ListFunction(string idBase) : base(idBase)
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
            if (parameters is ExpressionToken && !isFinal)
                return UnParsed(parameters);

            if (!(parameters is ListToken))
                parameters = parameters.Evaluate(substitutions, isFinal);

            if (!(parameters is ListToken listToken))
                throw new Exception($"Token must be list for '{Name}'");

            return PerformOnList(listToken, substitutions, isFinal);
        }

        protected abstract IToken PerformOnList(ListToken parameters, TokenTreeList substitutions, bool isFinal);
    }
}
