namespace TextParser.Tokens.Interfaces
{
    public interface IKeyToken : IToken
    {
        bool Matches(string text);
    }
}
