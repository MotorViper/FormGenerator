using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    public class CaseFunction : ListFunction
    {
        public CaseFunction() : base("CASE")
        {
        }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        protected override IToken PerformOnList(ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            int count = listToken.Count;

            IToken first = listToken[0];
            if (first is ExpressionToken)
                return UnParsed(listToken);

            for (int i = 1; i < count - 1; i += 2)
            {
                IToken second = listToken[i];
                if (second is ExpressionToken)
                    return UnParsed(listToken);

                if (first.ToString() == second.ToString())
                    return listToken[i + 1];
            }

            return count % 2 == 0 ? listToken[count - 1] : new NullToken();
        }
    }
}
