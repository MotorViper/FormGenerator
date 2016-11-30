using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Creates a list of tokens representing a range of:
    /// 1 .. P1 if only one parameter.
    /// P1 .. P2 if two parameters.
    /// P1 .. P2 with offset P3 if three parameters.
    /// </summary>
    public class RangeFunction : BaseFunction
    {
        public const string ID = "RANGE";

        /// <summary>
        /// Constructor.
        /// </summary>
        public RangeFunction() : base(ID)
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
            ListToken list = parameters as ListToken;
            if (list == null)
            {
                list = new ListToken();
                list.Tokens.Add(parameters);
            }

            List<IToken> parameterList = new List<IToken>();
            foreach (IToken item in list)
            {
                IToken toAdd = item;
                if (toAdd is ExpressionToken && !isFinal)
                    return UnParsed(parameters);
                parameterList.Add(toAdd);
            }

            ListToken result = new ListToken();
            switch (parameterList.Count)
            {
                case 1:
                    for (int i = 1; i <= parameterList[0].Convert<int>(); ++i)
                        result.Tokens.Add(new IntToken(i));
                    break;
                case 2:
                    for (int i = parameterList[0].Convert<int>(); i <= parameterList[1].Convert<int>(); ++i)
                        result.Tokens.Add(new IntToken(i));
                    break;
                case 3:
                    for (int i = parameterList[0].Convert<int>(); i <= parameterList[1].Convert<int>(); i += parameterList[2].Convert<int>())
                        result.Tokens.Add(new IntToken(i));
                    break;
                default:
                    throw new Exception($"Must have between 1 and 3 values for '{ID}': {parameters}");
            }
            return result;
        }
    }
}
