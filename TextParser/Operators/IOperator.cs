using TextParser.Tokens;

namespace TextParser.Operators
{
    public interface IOperator
    {
        string Text { get; }
        IToken Simplify(IToken first, IToken last);
        IToken Evaluate(IToken first, IToken last, TokenTreeList parameters);
    }
}
