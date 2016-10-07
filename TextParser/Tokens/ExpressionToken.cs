using System;
using Helpers;
using TextParser.Operators;

namespace TextParser.Tokens
{
    public class ExpressionToken : BaseToken
    {
        private readonly Lazy<ILogging> _logger = IOCContainer.Instance.LazyResolve<ILogging>();

        public ExpressionToken(IToken first, IOperator op, IToken second = null)
        {
            First = first;
            Operator = op;
            Second = second;
        }

        private ILogging Logger => _logger.Value;

        public IToken First { get; }

        public bool NeedsSecond
        {
            get
            {
                ExpressionToken expression = Second as ExpressionToken;
                return expression?.NeedsSecond ?? Second == null;
            }
        }

        public IOperator Operator { get; }
        public IToken Second { get; private set; }
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
            IToken simple = Simplify();
            if (!(simple is ExpressionToken))
                return simple.Convert<TTo>();
            throw new Exception("Could not convert ExpressionToken");
        }

        public override IToken Evaluate(TokenTreeList parameters, bool isFinal)
        {
            IToken result = Operator.Evaluate(First, Second, parameters, isFinal);
            Logger?.LogMessage($"{this} -> {result}", "Evaluate");
            return result;
        }

        public override IToken SubstituteParameters(TokenTree parameters)
        {
            return Operator.SubstituteParameters(First, Second, parameters);
        }
    }
}
