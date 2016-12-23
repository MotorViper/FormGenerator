using System;
using TextParser.Tokens;

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
            ListToken listToken = parameters as ListToken;
            if (listToken != null)
            {
                ListToken returnList = new ListToken();
                foreach (IToken token in listToken)
                    returnList.Add(Perform(token, substitutions, isFinal));
                return returnList;
            }

            ITypeToken typeToken = parameters as ITypeToken;
            if (typeToken != null)
                return new IntToken(typeToken.ToInt());

            if (parameters is NullToken)
                return parameters;

            throw new Exception($"Token must be list/item of tokens convertible to int for {Name}");
        }
    }
}
