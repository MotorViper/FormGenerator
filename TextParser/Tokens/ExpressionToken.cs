using System;
using Helpers;
using TextParser.Operators;

namespace TextParser.Tokens
{
    /// <summary>
    /// Token representing an expression.
    /// </summary>
    public class ExpressionToken : BaseToken
    {
        private readonly Lazy<ILogging> _logger = IOCContainer.Instance.LazyResolve<ILogging>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="first">First element of expression.</param>
        /// <param name="op">Expression operator.</param>
        /// <param name="second">Second element of expression.</param>
        public ExpressionToken(IToken first, IOperator op, IToken second = null)
        {
            First = first;
            Operator = op;
            Second = second;
        }

        public IToken First { get; }

        private ILogging Logger => _logger.Value;

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

        /// <summary>
        /// Converts the token to a list of tokens if possible and required.
        /// </summary>
        /// <returns>The list of tokens or the original token.</returns>
        public override IToken EvaluateList()
        {
            return Operator.EvaluateList(this);
        }

        public override IToken SubstituteParameters(TokenTree parameters)
        {
            IToken result = Operator.SubstituteParameters(First, Second, parameters);
            if (Logger != null)
            {
                string initial = ToString();
                string final = result.ToString();
                if (initial != final)
                    Logger.LogMessage($"{initial} -> {final}", "Substitute");
            }
            return result;
        }
    }
}
