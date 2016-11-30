using TextParser.Operators;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// The base class for all function classes.
    /// </summary>
    public abstract class BaseFunction : IFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The function name.</param>
        protected BaseFunction(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The function name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns true if the function can work with expression tokens.
        /// </summary>
        public virtual bool FinalCanBeExpression => false;

        /// <summary>
        /// Returns true if the function does comparisons, if so no pre-evaluation is done.
        /// </summary>
        public virtual bool IsComparisonFunction => false;

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public abstract IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal);

        /// <summary>
        /// The value to use if the evaluated parameter is an expression token.
        /// </summary>
        /// <param name="finalValue">The evaluated parameter.</param>
        /// <returns>The token to use.</returns>
        public virtual IToken ValueIfFinalValueIsExpression(ExpressionToken finalValue)
        {
            return new NullToken();
        }

        /// <summary>
        /// Returns the unparsed expression token.
        /// </summary>
        /// <param name="token">The parameters to the function.</param>
        /// <returns></returns>
        protected IToken UnParsed(IToken token)
        {
            return new ExpressionToken(null, new FunctionOperator(this), token);
        }
    }
}
