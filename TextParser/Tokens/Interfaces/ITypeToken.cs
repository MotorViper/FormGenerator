namespace TextParser.Tokens.Interfaces
{
    public interface ITypeToken : IToken
    {
        TokenType Type { get; }
        object Data { get; }
    }
}
