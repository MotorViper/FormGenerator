namespace TextParser.Tokens
{
    public interface IToken
    {
        string Text { get; }
        TTo Convert<TTo>();
        IToken Simplify();
        IToken Evaluate(TokenTreeList parameters, bool isFinal);
        IToken SubstituteParameters(TokenTree parameters);
    }
}
