using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    public class SubstitutionOperator : BaseOperator
    {
        public SubstitutionOperator() : base("$")
        {
        }

        public override bool CanBeBinary => false;
        public override bool CanBeUnary => true;

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
            if (parameters == null)
            {
                if (isFinal)
                    throw new Exception($"Operation {Text} must have parameters if final.");
                return new ExpressionToken(first, this, last);
            }

            if (first != null)
                throw new Exception($"Operation {Text} is unary.");
            if (last == null)
                throw new Exception($"Operation {Text} needs a variable.");

            IToken evaluated = last.Evaluate(parameters, isFinal);
            if (evaluated is ExpressionToken)
                return new ExpressionToken(null, this, evaluated);
            ListToken listToken = evaluated as ListToken;
            if (listToken != null && listToken.Value.Exists(x => x is ExpressionToken))
                return new ExpressionToken(null, this, evaluated);

            string text = evaluated.ToString();
            TokenTreeList found = parameters.FindMatches(text, true);
            ListToken result = new ListToken();
            foreach (TokenTree tokenTree in found)
            {
                bool debug = bool.Parse(tokenTree["Debug"] ?? "False");
                if (debug)
                    LogControl?.SetLogging(true);
                IToken token = tokenTree.Value.Evaluate(parameters, isFinal);
                if (token is NullToken)
                {
                    if (result.Value.Count <= 0 && isFinal)
                        result.Add(token);
                }
                else
                {
                    if (result.Value.Count == 1 && result.Value[0] is NullToken)
                        result.Value.Clear();
                    result.Add(token);
                }
                if (debug)
                    LogControl?.ResetLoggingToDefault();
            }

            if (result.Value.Count == 0)
                return new ExpressionToken(null, this, evaluated);
            return result.Value.Count == 1 ? result.Value[0] : result;
        }

        public override IToken SubstituteParameters(IToken first, IToken last, TokenTree parameters)
        {
            if (parameters == null)
                throw new Exception($"Operation {Text} must have parameters if final.");

            if (first != null)
                throw new Exception($"Operation {Text} is unary.");
            if (last == null)
                throw new Exception($"Operation {Text} needs a variable.");

            IToken evaluated = last.SubstituteParameters(parameters);
            if (evaluated is ExpressionToken)
                return new ExpressionToken(null, this, evaluated);
            ListToken listToken = evaluated as ListToken;
            if (listToken != null && listToken.Value.Exists(x => x is ExpressionToken))
                return new ExpressionToken(null, this, evaluated);

            string text = evaluated.ToString();
            TokenTree found = parameters.FindFirst(text);
            return found?.Value ?? new ExpressionToken(null, new SubstitutionOperator(), evaluated);
        }

        protected override IToken Evaluate(ITypeToken token)
        {
            return new ExpressionToken(null, this, token);
        }
    }
}
