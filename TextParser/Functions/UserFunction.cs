﻿using System;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    /// <summary>
    /// Process a user defined function.
    /// </summary>
    public class UserFunction : ListFunction
    {
        public string FunctionName { get; set; }

        public UserFunction() : base("FUNC")
        {
        }

        public String Key(int value)
        {
            return $"{FunctionName}_{value}";
        }

        /// <summary>
        /// Returns true if the function can work with expression tokens.
        /// </summary>
        public override bool FinalCanBeExpression => true;

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="listToken">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        protected override IToken PerformOnList(ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            int count = listToken.Count;
            if (count < 2)
                throw new Exception($"Must have at least 2 values for '{Name}': {listToken}");

            IToken method = listToken[0];

            if (isFinal)
            {
                TokenTree tree = new TokenTree();
                for (int i = 1; i < listToken.Count; ++i)
                {
                    IToken parameter = listToken[i];
                    tree.Children.Add(new TokenTree(Key(i), parameter));
                }
                method = method.SubstituteParameters(tree);
            }

            IToken parsed = method.Evaluate(substitutions, isFinal);
            if (parsed is ExpressionToken)
            {
                if (!isFinal)
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
