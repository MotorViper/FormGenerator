using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Splits the first parameter using the optional other parameters. 
    /// The second parameter is the character to split on - default space, and the third the maximum count - default unlimited.
    /// If the split character is a space this is treated as any number of spaces.
    /// </summary>
    public class SplitFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SplitFunction() : base("SP(LIT)")
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
            string toSplit;
            int maxCount = -1;
            string[] splitOn = {" ", "\t"};
            StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
            ListToken listToken = parameters as ListToken;

            if (listToken == null)
            {
                if (parameters is ExpressionToken)
                    return UnParsed(parameters);
                toSplit = parameters.ToString();
            }
            else
            {
                int count = listToken.Count;
                if (count != 2 && count != 3)
                    throw new Exception($"Must have 1, 2 or 3 values for '{Name}': {listToken}");

                if (listToken[0] is ExpressionToken)
                    return UnParsed(listToken);

                toSplit = listToken[0].ToString();
                if (count > 1)
                {
                    string text = listToken[1].ToString();
                    if (text != " ")
                    {
                        splitOn = new[] {text};
                        options = StringSplitOptions.None;
                    }
                }
                if (count > 2)
                    maxCount = listToken[2].ToInt();
            }

            ListToken result = new ListToken();
            string[] bits = maxCount <= 0 ? toSplit.Split(splitOn, options) : toSplit.Split(splitOn, maxCount, options);
            foreach (string bit in bits)
                result.Add(new StringToken(bit.Trim()));
            return result;
        }
    }
}
