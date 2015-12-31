using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public class SubstitutionOperator : BaseOperator
    {
        public SubstitutionOperator() : base("$")
        {
        }

        public override bool CanBeBinary => false;
        public override bool CanBeUnary => true;

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
            if (listToken != null && listToken.Tokens.Exists(x => x is ExpressionToken))
                return new ExpressionToken(null, this, evaluated);

            string text = evaluated.Text;
            TokenTreeList found = parameters.FindMatches(text, true);
            ListToken result = new ListToken();
            foreach (TokenTree tokenTree in found)
            {
                IToken token = tokenTree.Value.Evaluate(parameters, isFinal);
                if (!(token is NullToken))
                    result.Add(token);
            }

            if (result.Tokens.Count == 0)
                return new ExpressionToken(null, this, evaluated);
            return result.Tokens.Count == 1 ? result.Tokens[0] : result;
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
            if (listToken != null && listToken.Tokens.Exists(x => x is ExpressionToken))
                return new ExpressionToken(null, this, evaluated);

            string text = evaluated.Text;
            TokenTree found = parameters.FindFirst(text);
            return found?.Value ?? new ExpressionToken(null, new SubstitutionOperator(), last);
        }

        protected override IToken Evaluate(ITypeToken token)
        {
            return new ExpressionToken(null, this, token);
        }
    }
}
