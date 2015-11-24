using TextParser.Operators;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public abstract class BaseFunction : IFunction
    {
        protected BaseFunction(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public virtual bool FinalCanBeExpression => false;

        public abstract IToken Perform(IToken parameterList, TokenTreeList parameters, bool isFinal);

        protected IToken UnParsed(IToken token)
        {
            return new ExpressionToken(null, new FunctionOperator(this), token);
        }
    }
}
