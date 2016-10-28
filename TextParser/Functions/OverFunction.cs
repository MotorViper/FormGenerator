using System;
using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Converts a number of tokens into a single list that can be iterated over.
    /// </summary>
    public class OverFunction : BaseFunction
    {
        public const string ID = "OVER";

        public OverFunction() : base(ID)
        {
        }

        public override bool FinalCanBeExpression => true;

        public override IToken Perform(IToken parameterList, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = parameterList as ListToken;

            if (listToken == null)
                throw new Exception($"Last token must be list for '{ID}'");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            IToken iterand = lastList[count - 2];
            IToken method = lastList[count - 1];
            ListToken tokens = new ListToken();
            for (int i = 0; i < count - 2; i++)
            {
                IToken token = lastList[i];
                ListToken list = token as ListToken;
                if (list == null)
                {
                    list = new ListToken();
                    list.Tokens.Add(token);
                }
                foreach (IToken item in list.Tokens)
                {
                    TokenTree tree = new TokenTree();
                    tree.Children.Add(new TokenTree(iterand.Text, item));
                    IToken toCall = method.SubstituteParameters(tree);

                    IToken parsed = toCall.Evaluate(parameters, isFinal);
                    if (parsed is ExpressionToken)
                    {
                        if (!isFinal)
                            return UnParsed(listToken);
                    }
                    else if (!(parsed is NullToken))
                    {
                        tokens.Add(parsed);
                    }
                }
            }
            return tokens;
        }
    }
}
