using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    /// <summary>
    /// Index operator - List#n to get n-1th element of list (0 based).
    /// </summary>
    public class IndexOperator : BaseOperator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IndexOperator() : base("#")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            IToken evaluated = last.Evaluate(parameters, isFinal);
            if (evaluated == null)
                throw new Exception($"Second element of Operation {Text} is not unique.");

            ListToken evaluatedList = evaluated as ListToken;
            if (evaluatedList != null)
            {
                ListToken list = new ListToken();
                foreach (IToken item in evaluatedList.Tokens)
                    list.Tokens.Add(Evaluate(first, item, parameters, isFinal));
                return list;
            }

            IntToken intToken = evaluated as IntToken;
            if (intToken == null)
            {
                if (isFinal)
                    throw new Exception($"Operation {Text} must have integer second element.");
                return new ExpressionToken(first, new IndexOperator(), last);
            }

            IToken tokenList = first.Evaluate(parameters, isFinal);
            ListToken listToken = tokenList as ListToken;
            int index = intToken.Value;
            return listToken == null
                ? (index == 0 && tokenList is ITypeToken ? tokenList : new ExpressionToken(first, new IndexOperator(), intToken))
                : (listToken.Tokens.Count > index ? listToken.Tokens[index] : new NullToken());
        }
    }
}
