using System;

namespace TextParser.Tokens
{
    public class SubstitutionToken : OperatorToken
    {
        public SubstitutionToken() : base("$")
        {
        }

        public override bool CanBeBinary => false;
        public override bool CanBeUnary => true;

        public override TokenList Evaluate(IToken first, IToken last, TokenTreeList parameters)
        {
            if (first != null)
                throw new Exception($"Operation {Text} is unary.");
            if (last == null)
                throw new Exception($"Operation {Text} needs a variable.");

            IToken evaluated = last.Evaluate(parameters)[0];
            string text = evaluated.Text;
            TokenTreeList found = parameters.FindMatches(text, true);
            TokenList result = new TokenList();
            foreach (TokenTree tokenTree in found)
                result.AddRange(tokenTree.Value.Evaluate(parameters));
            if (result.Count == 0)
                result.Add(new ExpressionToken(null, this, evaluated));
            return result;
        }

        protected override TokenList Evaluate(ITypeToken token)
        {
            return new TokenList(new ExpressionToken(null, this, token));
        }
    }
}
