using System;
using TextParser.Tokens;

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

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

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
                result.Tokens.AddRange(tokens.Tokens);
            else
                result.Tokens.Add(firstList);
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
            IToken firstList = expression.First.EvaluateList();
            IToken lastList = expression.Second.EvaluateList();

            if (firstList == null || lastList == null)
                return expression;

            return AddToList(firstList, lastList);
        }
    }
}
