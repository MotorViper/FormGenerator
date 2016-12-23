using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Converts a string into a regular expression.
    /// </summary>
    public class RegexFunction : BaseFunction
    {
        public RegexFunction() : base("R(EGEX)")
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
            string text;
            RegExToken.RegexType regexType = RegExToken.RegexType.Wildcard;
            ListToken listToken = parameters as ListToken;
            if (listToken != null)
            {
                if (listToken.Count < 1 || listToken.Count > 2)
                    throw new Exception($"Must have 1 or 2 values for '{Name}': {listToken}");

                text = listToken[0].ToString();
                if (listToken.Count == 2)
                    switch (listToken[1].ToString())
                    {
                        case "R":
                            regexType = RegExToken.RegexType.Regex;
                            break;
                        case "S":
                            regexType = RegExToken.RegexType.Sql;
                            break;
                        case "W":
                            regexType = RegExToken.RegexType.Wildcard;
                            break;
                    }
            }
            else
            {
                text = parameters.ToString();
            }

            return new RegExToken(text, regexType);
        }
    }
}
