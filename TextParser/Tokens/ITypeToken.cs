namespace TextParser.Tokens
{
    public interface ITypeToken : IToken
    {
        TokenType Type { get; }
        object Data { get; }
    }
}
