using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    /// <summary>
    /// Processes elements in a property chain.
    /// </summary>
    public class ChainOperator : BaseOperator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChainOperator() : base(".")
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
            IToken firstEvaluated = first.Evaluate(parameters, isFinal);
            IToken lastEvaluated = last.Evaluate(parameters, isFinal);

            if (firstEvaluated == null || lastEvaluated == null)
                throw new Exception($"Operation {Text} is a binary operation.");

            return CreateChain(firstEvaluated, lastEvaluated);
        }

        private static IToken CreateChain(IToken firstList, IToken lastList)
        {
            ChainToken result = new ChainToken();
            if (firstList is ListToken tokens)
                result.Value.AddRange(tokens.Value);
            else
                result.Value.Add(firstList);
            result.Add(lastList);
            return result;
        }

        /// <summary>
        /// Evaluates the elements in a chain.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The original token by default.</returns>
        public override IToken EvaluateList(ExpressionToken expression)
        {
            IToken firstList = expression.First.Flatten();
            IToken lastList = expression.Second.Flatten();

            if (firstList == null || lastList == null)
                return expression;

            return CreateChain(firstList, lastList);
        }
    }
}
