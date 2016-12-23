using System.Collections.Generic;
using TextParser.Operators;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// The base class for all function classes.
    /// </summary>
    public abstract class BaseFunction : IFunction
    {
        private readonly string _idBase;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="idBase">The base string for creating the name and ids from. X(Y) -> X and XY, X -> X.</param>
        protected BaseFunction(string idBase)
        {
            _idBase = idBase;
            Name = idBase.Replace("(", "").Replace(")", "");
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
        /// Returns true if the function allows short circuit evaluation, if so no pre-evaluation is done.
        /// </summary>
        public virtual bool AllowsShortCircuit => false;

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
        /// The ids that can be used to reference the function.
        /// </summary>
        public virtual IEnumerable<string> Ids
        {
            get
            {
                int bracketPosition = _idBase.IndexOf('(');
                return bracketPosition > 0
                    ? new List<string> {_idBase.Substring(0, bracketPosition), Name}
                    : new List<string> {Name};
            }
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
