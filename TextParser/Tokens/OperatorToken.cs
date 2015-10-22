﻿using System;

namespace TextParser.Tokens
{
    public abstract class OperatorToken
    {
        protected OperatorToken(string text)
        {
            Text = text;
        }

        public virtual bool CanBeBinary => true;
        public virtual bool CanBeUnary => false;
        public string Text { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }

        public virtual TokenList Simplify(IToken first, IToken last)
        {
            if (first == null)
            {
                if (CanBeUnary)
                {
                    ITypeToken secondType = last?.Simplify()[0] as ITypeToken;
                    return secondType != null ? Evaluate(secondType) : new TokenList(new ExpressionToken(null, this, last));
                }
                throw new Exception($"Operation {Text} can not be unary.");
            }

            if (CanBeBinary)
            {
                ITypeToken firstType = first.Simplify()[0] as ITypeToken;
                ITypeToken secondType = last?.Simplify()[0] as ITypeToken;
                return firstType != null && secondType != null ? Evaluate(firstType, secondType) : new TokenList(new ExpressionToken(first, this, last));
            }

            throw new Exception($"Operation {Text} can not be binary.");
        }

        public virtual TokenList Evaluate(IToken first, IToken last, TokenTreeList parameters)
        {
            TokenList firstList = first?.Evaluate(parameters);
            TokenList lastList = last?.Evaluate(parameters);
            if (firstList != null && firstList.Count != 1)
                throw new Exception($"First element of Operation {Text} is not unique.");
            if (lastList != null && lastList.Count != 1)
                throw new Exception($"Second element of Operation {Text} is not unique.");

            return Simplify(firstList?[0], lastList?[0]);
        }

        protected virtual TokenList Evaluate(ITypeToken token)
        {
            throw new Exception($"Operation unary {Text} not supported for {token.Type}.");
        }

        protected virtual TokenList Evaluate(ITypeToken first, ITypeToken last)
        {
            throw new Exception($"Operation binary {Text} not supported for ({first.Type}, {last.Type}).");
        }

        public static OperatorToken CreateOperatorToken(string op)
        {
            switch (op)
            {
                case "#":
                    return new IndexToken();
                case "|":
                    return new ListToken();
                case "+":
                    return new PlusToken();
                case "*":
                case "x":
                case "×":
                    return new TimesToken();
                case "-":
                    return new MinusToken();
                case "/":
                case "÷":
                    return new DivideToken();
                default:
                    throw new Exception($"Did not understand '{op}' as an operator");
            }
        }
    }
}
