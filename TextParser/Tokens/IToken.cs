namespace TextParser.Tokens
{
    public interface IToken
    {
        string Text { get; }
        TTo Convert<TTo>();
        TokenList Simplify();
        TokenList Evaluate(TokenTreeList parameters);
    }
}
