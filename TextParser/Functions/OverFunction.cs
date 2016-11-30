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

        /// <summary>
        /// Constructor.
        /// </summary>
        public OverFunction() : base(ID)
        {
        }

        /// <summary>
        /// Returns true if the function can work with expression tokens.
        /// </summary>
        public override bool FinalCanBeExpression => true;

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            ListToken listToken = parameters as ListToken;

            if (listToken == null)
                throw new Exception($"Next token must be list for '{ID}'");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            IToken iterandKey = lastList[count - 2];
            IToken iterandIndex = null;
            ListToken iterandList = iterandKey as ListToken;
            if (iterandList != null)
            {
                if (iterandList.Tokens.Count != 2)
                    throw new Exception($"Can only have 1 or 2 iterators for '{ID}'");
                iterandIndex = iterandList.Tokens[1];
                iterandKey = iterandList.Tokens[0];
            }
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
                int index = 0;
                foreach (IToken item in list.Tokens)
                {
                    TokenTree tree = new TokenTree();
                    tree.Children.Add(new TokenTree(iterandKey.Text, item));
                    if (iterandIndex != null)
                        tree.Children.Add(new TokenTree(iterandIndex.Text, new IntToken(index)));
                    IToken toCall = method.SubstituteParameters(tree);
                    IToken parsed = toCall.Evaluate(substitutions, isFinal);
                    if (parsed is ExpressionToken)
                    {
                        if (!isFinal)
                            return UnParsed(listToken);
                    }
                    else if (!(parsed is NullToken))
                    {
                        tokens.Add(parsed);
                    }
                    ++index;
                }
            }
            return tokens;
        }
    }
}
