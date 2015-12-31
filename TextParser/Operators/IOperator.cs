using TextParser.Tokens;

namespace TextParser.Operators
{
    public interface IOperator
    {
        string Text { get; }
        IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal);
        IToken SubstituteParameters(IToken first, IToken second, TokenTree parameters);
    }
}
