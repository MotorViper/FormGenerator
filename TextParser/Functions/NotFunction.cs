using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    public class NotFunction : BaseFunction
    {
        /// <summary>
        /// Does a logical NOT on a token or list of tokens.
        /// </summary>
        public NotFunction() : base("NOT")
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
            IToken token = Not(parameters);
            return token.IsExpression ? UnParsed(parameters) : token;
        }

        private IToken Not(IToken parameters)
        {
            ListToken list = parameters as ListToken;
            if (list != null)
            {
                ListToken newList = new ListToken();
                foreach (IToken token in list)
                {
                    IToken notted = Not(token);
                    if (notted.IsExpression)
                        return list;
                    newList.Add(notted);
                }
                return newList;
            }

            if (parameters is ExpressionToken)
                return UnParsed(parameters);

            return new BoolToken(!parameters.ToBool());
        }
    }
}
