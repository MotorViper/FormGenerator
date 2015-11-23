using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class UserFunction : BaseFunction
    {
        public override bool FinalCanBeExpression => true;

        public override IToken Perform(IToken parameterList, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = parameterList as ListToken;

            if (listToken == null)
                throw new Exception("Last token must be list for FUNC");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            if (count != 2)
                throw new Exception($"Must have 2 values for FUNC: {listToken}");

            IToken parameter = lastList[1];
            IToken method = lastList[0];

            TokenTreeList treeList = parameters?.Clone() ?? new TokenTreeList();
            TokenTree tree = new TokenTree();
            tree.Children.Add(new TokenTree(new StringToken("Item"), parameter));
            treeList.Add(tree);
            IToken parsed = method.Evaluate(treeList, isFinal);
            if (parsed is ExpressionToken)
            {
                if (parameters == null)
                    return UnParsed(listToken);
            }
            else
            {
                return parsed;
            }
            return new NullToken();
        }
    }
}
