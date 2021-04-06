using Helpers;
using System;
using TextParser.Operators;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    /// <summary>
    /// Token representing an expression.
    /// </summary>
    public class ExpressionToken : BaseToken//, IValueToken, IKeyToken
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

        /// <summary>
        /// Whether this is token contains an expression.
        /// </summary>
        public override bool IsExpression => true;

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            // When it's a function operator it can be partially parsed and that causes the function name to be shown twice.
            if (First == null || (Operator is FunctionOperator op && op.Function != null))
                return $"({Operator.Text}{Second.ToString()})";
            return $"({First.ToString()}{Operator.Text}{Second.ToString()})";
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

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool()
        {
            IToken simple = Simplify();
            if (!(simple is ExpressionToken))
                return simple.ToBool();
            throw new Exception("Could not convert ExpressionToken");
        }

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt()
        {
            IToken simple = Simplify();
            if (!(simple is ExpressionToken))
                return simple.ToInt();
            throw new Exception("Could not convert ExpressionToken");
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble()
        {
            IToken simple = Simplify();
            if (!(simple is ExpressionToken))
                return simple.ToDouble();
            throw new Exception("Could not convert ExpressionToken");
        }

        /// <summary>
        /// Evaluates the token.
        /// </summary>
        /// <param name="parameters">The parameters to use for substitutions.</param>
        /// <param name="isFinal">Whether this is a final parse.</param>
        /// <returns></returns>
        protected override IToken Process(TokenTreeList parameters, bool isFinal)
        {
            IToken result = Operator.Evaluate(First, Second, parameters, isFinal);
            if (ToString() != result.ToString())
                Logger?.LogMessage($"{this} -> {result}", "Evaluate");
            return result;
        }

        //public bool Matches(string text)
        //{
        //    return ToString() == text;
        //}

        /// <summary>
        /// Converts the token to a list of tokens if possible and required.
        /// </summary>
        /// <returns>The list of tokens or the original token.</returns>
        public override IToken Flatten()
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

        //public override IToken ValueToken => this;
    }
}
