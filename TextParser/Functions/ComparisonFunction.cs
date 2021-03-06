﻿using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Function to do a comparison. Action depends on number of parameters:
    /// 2: If first matches second then true is returned otherwise false.
    /// 3: If first matches second then third is returned otherwise null.
    /// 4: If first matches second then third is returned otherwise fourth.
    /// 5: If first greater than second third is returned, if equal fourth is returned otherwise fifth is returned.
    /// </summary>
    public class ComparisonFunction : CheckedCountFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ComparisonFunction() : base("COMP(ARE)", 2, 5)
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
        protected override IToken PerformOnList(int count, ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            IToken first = listToken[0].Evaluate(substitutions, isFinal);
            IToken second = listToken[1].Evaluate(substitutions, isFinal);
            if (first is ExpressionToken || second is ExpressionToken)
                return UnParsed(listToken);

            int comparison;
            if (first is RegExToken)
            {
                if (count > 4)
                    throw new Exception($"Must have 2 to 4 values for '{Name}': {listToken}");
                comparison = first.HasMatch(second) ? 0 : 1;
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
                case 2:
                    result = new BoolToken(comparison == 0);
                    break;
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
