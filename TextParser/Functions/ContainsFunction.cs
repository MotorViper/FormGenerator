﻿using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Checks if a list contains an instance of a token.
    /// If there are two items in the parameter list then either a string contains or a list contains is done dependant on the type of the first item.
    /// If there are more than two items then the initial list is checked to see if it contains the last item.
    /// </summary>
    public class ContainsFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ContainsFunction() : base("CONT(AINS)")
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
                throw new Exception($"Token must be list for '{Name}'");

            int count = listToken.Count;
            IToken toFind = listToken[count - 1];
            if (toFind is ExpressionToken)
            {
                if (isFinal)
                    throw new Exception($"Could not find value for {toFind}");
                return UnParsed(listToken);
            }

            if (count == 2)
            {
                IToken token = listToken[0];
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                if (token is StringToken)
                    return new BoolToken(token.ToString().Contains(toFind.ToString()));
                ListToken list = token as ListToken;
                if (list != null)
                    return new BoolToken(list.Contains(toFind));
            }

            for (int i = 0; i < count - 1; i++)
            {
                IToken token = listToken[i];
                if (token is ExpressionToken)
                    return UnParsed(listToken);
                if (token.ToString() == toFind.ToString())
                    return new BoolToken(true);
            }
            return new BoolToken(false);
        }
    }
}
