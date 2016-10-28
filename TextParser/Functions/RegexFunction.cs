using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Converts a string into a regular expression.
    /// </summary>
    public class RegexFunction : BaseFunction
    {
        public const string ID = "R";

        public RegexFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken dataToken, TokenTreeList parameters, bool isFinal)
        {
            string text = null;
            RegExToken.RegexType regexType = RegExToken.RegexType.Wildcard;
            ListToken listToken = dataToken as ListToken;
            if (listToken != null)
            {
                List<IToken> list = listToken.Tokens;
                if (list.Count < 1 || list.Count > 2)
                    throw new Exception($"Must have 1 or 2 values for '{ID}': {listToken}");

                text = list[0].Text;
                if (list.Count == 2)
                    switch (list[1].Text)
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
                text = dataToken.Text;
            }

            return new RegExToken(text, regexType);
        }
    }
}
