using TextParser.Tokens;

namespace TextParser.Functions
{
    public class FlattenFunction : BaseFunction
    {
        public FlattenFunction() : base("F(LATTEN)")
        {
        }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            return parameters.IsExpression ? UnParsed(parameters) : parameters.Flatten();
        }
    }
}
