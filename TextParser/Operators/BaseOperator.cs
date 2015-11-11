﻿using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public abstract class BaseOperator : IOperator
    {
        protected BaseOperator(string text)
        {
            Text = text;
        }

        public virtual bool CanBeBinary => true;
        public virtual bool CanBeUnary => false;
        public string Text { get; }

        public virtual IToken Simplify(IToken first, IToken last)
        {
            if (first == null)
            {
                if (CanBeUnary)
                {
                    ITypeToken secondType = last?.Simplify() as ITypeToken;
                    return secondType != null ? Evaluate(secondType) : new ExpressionToken(null, this, last);
                }
                throw new Exception($"Operation {Text} can not be unary.");
            }

            if (CanBeBinary)
            {
                ITypeToken firstType = first.Simplify() as ITypeToken;
                ITypeToken secondType = last?.Simplify() as ITypeToken;
                return firstType != null && secondType != null ? Evaluate(firstType, secondType) : new ExpressionToken(first, this, last);
            }

            throw new Exception($"Operation {Text} can not be binary.");
        }

        public virtual IToken Evaluate(IToken first, IToken last, TokenTreeList parameters)
        {
            return Simplify(first?.Evaluate(parameters), last?.Evaluate(parameters));
        }

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

        protected virtual IToken Evaluate(ITypeToken token)
        {
            throw new Exception($"Operation unary {Text} not supported for {token.Type}.");
        }

        protected virtual IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            throw new Exception($"Operation binary {Text} not supported for ({first.Type}, {last.Type}).");
        }

        public static BaseOperator CreateOperatorToken(string op)
        {
            switch (op)
            {
                case "#":
                    return new IndexOperator();
                case ":":
                    return new FunctionOperator();
                case "|":
                    return new ListOperator();
                case "+":
                    return new PlusOperator();
                case "*":
                case "x":
                case "×":
                    return new TimesOperator();
                case "-":
                    return new MinusOperator();
                case "/":
                case "÷":
                    return new DivideOperator();
                default:
                    throw new Exception($"Did not understand '{op}' as an operator");
            }
        }
    }
}