using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Converts a number of tokens into a single list that can be iterated over.
    /// </summary>
    public class OverFunction : ListFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OverFunction() : base("OVER")
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
        protected override IToken PerformOnList(ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            //int count = listToken.Count;
            //IToken iterandKey = listToken[count - 2];
            //IToken iterandIndex = null;
            //if (iterandKey is ListToken iterandList)
            //{
            //    if (iterandList.Count != 2)
            //        throw new Exception($"Can only have 1 or 2 iterators for '{Name}'");
            //    iterandIndex = iterandList[1];
            //    iterandKey = iterandList[0];
            //}
            //IToken method = listToken[count - 1];
            //ITreeToken originalParent = method.Parent;
            //ListToken tokens = new ListToken();
            //for (int i = 0; i < count - 2; i++)
            //{
            //    IToken token = listToken[i];
            //    ListToken list = token as ListToken ?? new ListToken(token) { Parent = listToken.Parent };
            //    int index = 0;
            //    ITreeToken parent = originalParent.Clone();
            //    foreach (IToken item in list)
            //    {
            //        method.Parent = parent;
            //        parent.ReplaceChild(new PairToken((StringToken)iterandKey.ToString(), item));
            //        if (iterandIndex != null)
            //            parent.ReplaceChild(new PairToken((StringToken)iterandIndex.ToString(), new IntToken(index)));
            //        IToken parsed = method.Evaluate(isFinal);
            //        if (parsed is ExpressionToken)
            //        {
            //            if (!isFinal)
            //                return UnParsed(listToken);
            //        }
            //        else if (parsed != null)
            //        {
            //            tokens.Add(parsed);
            //        }
            //        ++index;
            //    }
            //}
            //method.Parent = originalParent;
            int count = listToken.Count;
            IToken iterandKey = listToken[count - 2];
            IToken iterandIndex = null;
            ListToken iterandList = iterandKey as ListToken;
            if (iterandList != null)
            {
                if (iterandList.Count != 2)
                    throw new Exception($"Can only have 1 or 2 iterators for '{Name}'");
                iterandIndex = iterandList[1];
                iterandKey = iterandList[0];
            }
            IToken method = listToken[count - 1];
            ListToken tokens = new ListToken();
            for (int i = 0; i < count - 2; i++)
            {
                IToken token = listToken[i];
                ListToken list = token as ListToken ?? new ListToken(token);
                if (list.IsExpression)
                {
                    IToken parsed = list.Evaluate(substitutions, isFinal);
                    if (!parsed.IsExpression)
                    {
                        if (parsed is ListToken parsedList && parsedList.Count == 1)
                            parsed = parsedList.Value[0];
                        if (!(parsed is NullToken))
                            list = parsed is ListToken token1 ? token1 : new ListToken(parsed);
                    }
                }
                int index = 0;
                foreach (IToken item in list)
                {
                    TokenTree tree = new TokenTree();
                    tree.Children.Add(new TokenTree(iterandKey.ToString(), item));
                    if (iterandIndex != null)
                        tree.Children.Add(new TokenTree(iterandIndex.ToString(), new IntToken(index)));
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
