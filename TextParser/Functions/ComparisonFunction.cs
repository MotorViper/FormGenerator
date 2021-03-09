using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Function to do a comparison. Action depends on number of parameters:
    /// 3: If first matches second then third is returned otherwise null.
    /// 4: If first matches second then third is returned otherwise fourth.
    /// 5: If first greater than second third is returned, if equal fourth is returned otherwise fifth is returned.
    /// </summary>
    public class ComparisonFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ComparisonFunction() : base("COMP(ARE)")
        {
        }

        /// <summary>
        /// Returns true if the function allows short circuit evaluation, if so no pre-evaluation is done.
        /// </summary>
        public override bool AllowsShortCircuit => true;

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
                throw new Exception($"Last token must be list for '{Name}'");

            int count = listToken.Count;

            if (count < 3 || count > 5)
                throw new Exception($"Must have between 3 and 5 values for '{Name}': {listToken}");

            IToken first = listToken[0].Evaluate(substitutions, isFinal);
            IToken second = listToken[1].Evaluate(substitutions, isFinal);
            if (first is ExpressionToken || second is ExpressionToken)
                return UnParsed(listToken);

            int comparison;
            if (first is RegExToken)
            {
                if (count > 4)
                    throw new Exception($"Must have 3 or 4 values for '{Name}': {listToken}");
                comparison = first.Contains(second.ToString()) ? 0 : 1;
            }
            else
            {
                comparison = (first is IntToken || first is DoubleToken) && (second is IntToken || second is DoubleToken)
                    ? first.ToDouble().CompareTo(second.ToDouble())
                    : first.ToString().CompareTo(second.ToString());
            }

            IToken result;
            switch (count)
            {
                case 3:
                    result = comparison == 0 ? listToken[2] : new NullToken();
                    break;
                case 4:
                    result = comparison == 0 ? listToken[2] : listToken[3];
                    break;
                default:
                    result = comparison == 1 ? listToken[2] : comparison == 0 ? listToken[3] : listToken[4];
                    break;
            }
            return result.Evaluate(substitutions, isFinal);
        }
    }
}
