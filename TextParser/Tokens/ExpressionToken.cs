﻿using System;

namespace TextParser.Tokens
{
    public class ExpressionToken : BaseToken
    {
        public OperatorToken Operator { get; }
        public IToken First { get; }
        public IToken Second { get; private set; }

        public ExpressionToken(IToken first, OperatorToken op, IToken second = null)
        {
            First = first;
            Operator = op;
            Second = second;
        }

        public bool NeedsSecond
        {
            get
            {
                ExpressionToken expression = Second as ExpressionToken;
                return expression?.NeedsSecond ?? Second == null;
            }
        }

        public override string Text => First == null ? $"({Operator.Text}{Second.Text})" : $"({First.Text}{Operator.Text}{Second.Text})";

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "ExpressionToken: " + Text;
        }

        public ExpressionToken SetSecond(IToken token)
        {
            ExpressionToken expression = Second as ExpressionToken;
            if (expression != null)
                expression.SetSecond(token);
            else if (Second == null)
                Second = token;
            else
                throw new Exception("Second already set");
            return this;
        }

        public override TTo Convert<TTo>()
        {
            IToken simple = Simplify()[0];
            if (!(simple is ExpressionToken))
                return simple.Convert<TTo>();
            throw new Exception("Could not convert ExpressionToken");
        }

        public override TokenList Simplify()
        {
            return Operator.Simplify(First, Second);
        }

        public override TokenList Evaluate(TokenTreeList parameters)
        {
            return Operator.Evaluate(First, Second, parameters);
        }
    }
}
