using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    /// <summary>
    /// Adds elements to the end of a list.
    /// </summary>
    public class ListOperator : BaseOperator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ListOperator() : base("|")
        {
        }

        /// <summary>
        /// Evaluates an operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <param name="parameters">Any substitution parameters.</param>
        /// <param name="isFinal">Whether this is the final (output) call.</param>
        /// <returns>The evaluated value.</returns>
        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal)
        {
            IToken firstList = first.Evaluate(parameters, isFinal);
            IToken lastList = last.Evaluate(parameters, isFinal);

            if (firstList == null || lastList == null)
                throw new Exception($"Operation {Text} is a binary operation.");

            return AddToList(firstList, lastList);
        }

        private static IToken AddToList(IToken firstList, IToken lastList)
        {
            ListToken result = new ListToken();
            ListToken tokens = firstList as ListToken;
            if (tokens != null)
                result.Value.AddRange(tokens.Value);
            else
                result.Value.Add(firstList);
            result.Add(lastList);
            return result;
        }

        /// <summary>
        /// Converts an expression token to a list of tokens if possible and required.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The original token by default.</returns>
        public override IToken EvaluateList(ExpressionToken expression)
        {
            IToken firstList = expression.First.Flatten();
            IToken lastList = expression.Second.Flatten();

            if (firstList == null || lastList == null)
                return expression;

            return AddToList(firstList, lastList);
        }
    }
}
