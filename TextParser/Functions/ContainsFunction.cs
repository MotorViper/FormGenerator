using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Checks if a list contains an instance of a token.
    /// </summary>
    public class ContainsFunction : BaseFunction
    {
        public const string ID = "CONTAINS";

        /// <summary>
        /// Constructor.
        /// </summary>
        public ContainsFunction() : base(ID)
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
            ListToken listToken = parameters as ListToken;

            if (listToken == null)
                throw new Exception($"Token must be list for '{ID}'");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            IToken toFind = lastList[count - 1];
            if (toFind is ExpressionToken)
            {
                if (isFinal)
                    throw new Exception($"Could not find value for {toFind}");
                return UnParsed(listToken);
            }

            for (int i = 0; i < count - 1; i++)
            {
                IToken token = lastList[i];
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                ListToken list = token as ListToken;
                if (list != null && list.Tokens.Contains(toFind))
                    return new BoolTooken(true);
                if (token.Text == toFind.Text)
                    return new BoolTooken(true);
            }
            return new BoolTooken(false);
        }
    }
}
