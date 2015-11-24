using TextParser.Tokens;

namespace TextParser.Functions
{
    public interface IFunction
    {
        bool FinalCanBeExpression { get; }
        string Name { get; }
        IToken Perform(IToken parameterList, TokenTreeList parameters, bool isFinal);
    }
}
