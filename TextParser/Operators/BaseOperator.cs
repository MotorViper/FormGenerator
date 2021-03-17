using System;
using Helpers;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    /// <summary>
    /// Base class for all operators.
    /// </summary>
    public abstract class BaseOperator : IOperator
    {
        private static readonly Lazy<ILogControl> s_control = IOCContainer.Instance.LazyResolve<ILogControl>();

        protected BaseOperator(string text)
        {
            Text = text;
        }

        public virtual bool CanBeBinary => true;
        public virtual bool CanBeUnary => false;

        protected ILogControl LogControl => s_control.Value;

        public virtual string Text { get; }

        /// <summary>
        /// Evaluates an operator expression.
        /// </summary>
        /// <param name="firstToken">The first value.</param>
        /// <param name="lastToken">The second value.</param>
        /// <param name="parameters">Any substitution parameters.</param>
        /// <param name="isFinal">Whether this is the final (output) call.</param>
        /// <returns>The evaluated value.</returns>
        public virtual IToken Evaluate(IToken firstToken, IToken lastToken, TokenTreeList parameters, bool isFinal)
        {
            IToken last = lastToken?.Evaluate(parameters, isFinal);
            if (last == null)
                throw new Exception($"Operation {Text} must have parameters.");
            ITypeToken lastTypeToken = last as ITypeToken;

            IToken first = firstToken?.Evaluate(parameters, isFinal);
            if (first == null)
            {
                if (!CanBeUnary)
                    throw new Exception($"Operation {Text} can not be unary.");
                if ((last as NullToken) != null)
                    return last;
                return lastTypeToken == null || last.IsExpression ? new ExpressionToken(null, this, last) : Evaluate(lastTypeToken);
            }

            if (!CanBeBinary)
                throw new Exception($"Operation {Text} can not be binary.");

            if ((first as NullToken) != null)
                return last;

            if ((last as NullToken) != null)
                return first;

            ITypeToken firstTypeToken = first as ITypeToken;
            return firstTypeToken == null || lastTypeToken == null || first.IsExpression || last.IsExpression
                ? new ExpressionToken(first, this, last)
                : Evaluate(firstTypeToken, lastTypeToken);
        }

        public virtual IToken SubstituteParameters(IToken firstToken, IToken lastToken, TokenTree parameters)
        {
            IToken first = firstToken?.SubstituteParameters(parameters);
            IToken last = lastToken?.SubstituteParameters(parameters);
            return new ExpressionToken(first, this, last);
        }

        /// <summary>
        /// Converts an expression token to a list of tokens if possible and required.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The original token by default.</returns>
        public virtual IToken EvaluateList(ExpressionToken expression)
        {
            return expression;
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

        /// <summary>
        /// Evaluates a unary operator expression.
        /// </summary>
        /// <param name="token">The value to apply the operator to.</param>
        /// <returns>The evaluated value.</returns>
        protected virtual IToken Evaluate(ITypeToken token)
        {
            throw new Exception($"Operation unary {Text} not supported for {token.GetType().Name}.");
        }

        /// <summary>
        /// Evaluates a binary operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <returns>The evaluated value.</returns>
        protected virtual IToken Evaluate(ITypeToken first, ITypeToken last)
        {
            throw new Exception($"Operation binary {Text} not supported for ({first.GetType().Name}, {last.GetType().Name}).");
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
                case ",":
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
